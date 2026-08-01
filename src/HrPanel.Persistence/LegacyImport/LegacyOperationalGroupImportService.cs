using HrPanel.Application.Common.Abstractions.LegacyImport;
using HrPanel.Application.Dtos.LegacyImport;
using HrPanel.Domain.Employment;
using HrPanel.Domain.Organization;
using HrPanel.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HrPanel.Persistence.LegacyImport;

public sealed class LegacyOperationalGroupImportService: ILegacyOperationalGroupImportService
{
    private readonly HrDbContext _dbContext;

    public LegacyOperationalGroupImportService(HrDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LegacyOperationalGroupImportResult> ImportAsync(Guid batchId,CancellationToken cancellationToken = default)
    {
        if (batchId == Guid.Empty)
        {
            throw new ArgumentException("The legacy import batch ID cannot be empty.", nameof(batchId));
        }

        var executionStrategy = _dbContext.Database.CreateExecutionStrategy();

        LegacyOperationalGroupImportResult? result = null;

        await executionStrategy.ExecuteAsync(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            _dbContext.ChangeTracker.Clear();

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            try
            {
                result = await ImportInternalAsync(batchId,cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });

        return result ?? throw new InvalidOperationException( "The operational-group import did not produce a result.");
    }

    private async Task<LegacyOperationalGroupImportResult> ImportInternalAsync(Guid batchId,CancellationToken cancellationToken)
    {
        var warnings = new List<string>();

        var sourceRows = await _dbContext.LegacyEmployeeImportRows
            .AsNoTracking()
            .Where(row => row.BatchId == batchId)
            .OrderBy(row => row.SourceRowNumber)
            .ToListAsync(cancellationToken);

        var pilotRows = sourceRows
            .Select(row =>
            {
                var groupCode = NormalizePilotCode(row.Pilot);

                return new PilotSourceRow(
                    row.SourceRowNumber,
                    row.Pilot,
                    groupCode,
                    row.ImportedEmployeeId);
            })
            .Where(row => row.GroupCode is not null)
            .Select(row => row with
            {
                GroupCode = row.GroupCode!
            })
            .ToList();

        var distinctGroupCodes = pilotRows
            .Select(row => row.GroupCode!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(code => code, StringComparer.OrdinalIgnoreCase)
            .ToList();

        /*
         * Step 1:
         * Find existing group definitions
         */
        var existingGroups = distinctGroupCodes.Count == 0
            ? new List<OperationalGroup>()
            : await _dbContext.OperationalGroups
                .Where(group =>
                    distinctGroupCodes.Contains(group.Code))
                .ToListAsync(cancellationToken);

        var groupsByCode = existingGroups
            .ToDictionary(
                group => group.Code,
                StringComparer.OrdinalIgnoreCase);

        var groupsCreated = 0;

        /*
         * Step 2:
         * Create definitions that do not already exist
         */
        foreach (var groupCode in distinctGroupCodes)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (groupsByCode.ContainsKey(groupCode))
            {
                continue;
            }

            var group = OperationalGroup.Create(
                groupCode,
                CreateGroupName(groupCode),
                OperationalGroupType.ContactCenterAgentGroup);

            _dbContext.OperationalGroups.Add(group);
            groupsByCode.Add(groupCode, group);

            groupsCreated++;
        }

        if (groupsCreated > 0)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /*
         * Step 3:
         * Load normalized employments for employees referenced by PILOT rows.
         */
        var employeeIds = pilotRows
            .Where(row => row.ImportedEmployeeId.HasValue)
            .Select(row => row.ImportedEmployeeId!.Value)
            .Distinct()
            .ToList();

        var employments = employeeIds.Count == 0
            ? new List<Employment>()
            : await _dbContext.Employments
                .AsNoTracking()
                .Where(employment =>
                    employeeIds.Contains(employment.EmployeeId))
                .ToListAsync(cancellationToken);

        var employmentsByEmployeeId = employments
            .GroupBy(employment => employment.EmployeeId)
            .ToDictionary(
                group => group.Key,
                group => group
                    .OrderByDescending(employment =>
                        employment.EndDate is null)
                    .ThenByDescending(employment =>
                        employment.StartDate)
                    .ThenByDescending(employment =>
                        employment.Id)
                    .ToArray());

        var employmentIds = employments
            .Select(employment => employment.Id)
            .Distinct()
            .ToList();

        /*
         * Step 4:
         * Load existing memberships to make repeated execution idempotent.
         */
        var existingAssignments = employmentIds.Count == 0
            ? new List<EmployeeOperationalGroupAssignment>()
            : await _dbContext.EmployeeOperationalGroupAssignments
                .AsNoTracking()
                .Where(assignment =>
                    employmentIds.Contains(assignment.EmploymentId))
                .ToListAsync(cancellationToken);

        var existingMemberships = existingAssignments
            .Select(assignment => new MembershipKey(
                assignment.EmploymentId,
                assignment.OperationalGroupId,
                assignment.EffectiveFrom))
            .ToHashSet();

        /*
         * The filtered database index permits only one current primary group
         * for each employment.
         */
        var employmentsWithCurrentPrimaryGroup = existingAssignments
            .Where(assignment =>
                assignment.EffectiveTo is null &&
                assignment.IsPrimary)
            .Select(assignment => assignment.EmploymentId)
            .ToHashSet();

        var assignmentsCreated = 0;
        var assignmentsSkippedExisting = 0;
        var rowsSkippedWithoutEmployment = 0;

        /*
         * Step 5:
         * Resolve each PILOT row and create its employment membership
         */
        foreach (var pilotRow in pilotRows)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!pilotRow.ImportedEmployeeId.HasValue)
            {
                rowsSkippedWithoutEmployment++;

                warnings.Add(
                    $"Row {pilotRow.SourceRowNumber}: PILOT " +
                    $"'{pilotRow.OriginalPilot}' was not imported because " +
                    "the staging row has no ImportedEmployeeId.");

                continue;
            }

            if (!employmentsByEmployeeId.TryGetValue(
                    pilotRow.ImportedEmployeeId.Value,
                    out var employeeEmployments) ||
                employeeEmployments.Length == 0)
            {
                rowsSkippedWithoutEmployment++;

                warnings.Add(
                    $"Row {pilotRow.SourceRowNumber}: PILOT " +
                    $"'{pilotRow.OriginalPilot}' was not imported because " +
                    $"employee {pilotRow.ImportedEmployeeId.Value} has no " +
                    "normalized employment.");

                continue;
            }

            var employment = employeeEmployments[0];

            var operationalGroup = groupsByCode[pilotRow.GroupCode!];

            var effectiveFrom = employment.StartDate;

            var membershipKey = new MembershipKey(employment.Id,operationalGroup.Id,effectiveFrom);

            if (existingMemberships.Contains(membershipKey))
            {
                assignmentsSkippedExisting++;
                continue;
            }


            if (employmentsWithCurrentPrimaryGroup.Contains(
                    employment.Id))
            {
                assignmentsSkippedExisting++;

                warnings.Add(
                    $"Row {pilotRow.SourceRowNumber}: employment " +
                    $"{employment.Id} already has a current primary " +
                    $"operational-group assignment. PILOT " +
                    $"'{pilotRow.OriginalPilot}' was skipped.");

                continue;
            }

            var assignment = EmployeeOperationalGroupAssignment.Create(employment.Id,operationalGroup.Id,effectiveFrom,isPrimary: true);

            _dbContext.EmployeeOperationalGroupAssignments.Add(assignment);

            existingMemberships.Add(membershipKey);

            employmentsWithCurrentPrimaryGroup.Add(employment.Id);

            assignmentsCreated++;
        }

        if (assignmentsCreated > 0)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return new LegacyOperationalGroupImportResult
        {
            BatchId = batchId,
            SourceRowsReviewed = sourceRows.Count,
            RowsWithPilot = pilotRows.Count,
            DistinctGroupsFound = distinctGroupCodes.Count,
            GroupsCreated = groupsCreated,
            GroupsReused =
                distinctGroupCodes.Count - groupsCreated,
            AssignmentsCreated = assignmentsCreated,
            AssignmentsSkippedExisting =
                assignmentsSkippedExisting,
            RowsSkippedWithoutEmployment =
                rowsSkippedWithoutEmployment,
            Warnings = warnings.ToArray()
        };
    }
    private static string? NormalizePilotCode(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim().Replace('_', ' ');

        while (normalized.Contains("  ",StringComparison.Ordinal))
        {
            normalized = normalized.Replace("  "," ",StringComparison.Ordinal);
        }


        // اگر متغیرهای قدیمی رایج در دسته‌ای آینده ظاهر شوند، آنها را نادیده بگیرید

        if (normalized.Equals(
                "NULL",
                StringComparison.OrdinalIgnoreCase) ||
            normalized.Equals(
                "N/A",
                StringComparison.OrdinalIgnoreCase) ||
            normalized.Equals(
                "-",
                StringComparison.OrdinalIgnoreCase) ||
            normalized.Equals(
                "***",
                StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return normalized.ToUpperInvariant().Replace(' ', '_');
    }

    private static string CreateGroupName( string groupCode)
    {
        return groupCode.Replace('_', ' ');
    }

    private sealed record PilotSourceRow(int SourceRowNumber,string? OriginalPilot,string? GroupCode,long? ImportedEmployeeId);

    private readonly record struct MembershipKey(long EmploymentId,long OperationalGroupId,DateOnly EffectiveFrom);
}