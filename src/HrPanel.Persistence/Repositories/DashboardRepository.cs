using HrPanel.Application.Common.Abstractions.Persistence;
using HrPanel.Application.Dtos.Dashboard;
using HrPanel.Domain.Assets;
using HrPanel.Domain.Employment;
using HrPanel.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace HrPanel.Persistence.Repositories;

internal sealed class DashboardRepository(HrDbContext dbContext) : IDashboardRepository
{
    public async Task<DashboardDto> GetAsync(DateTime recentFrom, CancellationToken cancellationToken = default)
    {
        var total = await dbContext.Employees.AsNoTracking().CountAsync(cancellationToken);
        var active = await dbContext.Employees.AsNoTracking().CountAsync(x => x.IsActive, cancellationToken);
        var newEmployees = await dbContext.Employees.AsNoTracking().CountAsync(x => x.CreatedAt >= recentFrom, cancellationToken);

        var employmentStatusRows = await dbContext.Employments.AsNoTracking()
            .Where(x => x.EndDate == null)
            .GroupBy(x => x.EmploymentStatus.NameFa)
            .Select(x => new { Label = x.Key, Value = x.Count() })
            .OrderByDescending(x => x.Value).Take(8).ToListAsync(cancellationToken);
        var employmentStatuses = employmentStatusRows
            .Select(x => new DashboardMetricDto(x.Label, x.Value))
            .ToArray();

        var employmentTypeRows = await dbContext.Employments.AsNoTracking()
            .Where(x => x.EndDate == null)
            .GroupBy(x => x.EmploymentType.NameFa)
            .Select(x => new { Label = x.Key, Value = x.Count() })
            .OrderByDescending(x => x.Value).Take(8).ToListAsync(cancellationToken);
        var employmentTypes = employmentTypeRows
            .Select(x => new DashboardMetricDto(x.Label, x.Value))
            .ToArray();

        var organizationUnitRows = await dbContext.EmployeeAssignments.AsNoTracking()
            .Where(x => x.EffectiveTo == null && x.Context == AssignmentContext.Hr && x.OrganizationUnit != null)
            .GroupBy(x => x.OrganizationUnit!.NameFa)
            .Select(x => new { Label = x.Key, Value = x.Count() })
            .OrderByDescending(x => x.Value).Take(8).ToListAsync(cancellationToken);
        var organizationUnits = organizationUnitRows
            .Select(x => new DashboardMetricDto(x.Label, x.Value))
            .ToArray();

        var workLocationRows = await dbContext.EmployeeAssignments.AsNoTracking()
            .Where(x => x.EffectiveTo == null && x.Context == AssignmentContext.Hr && x.WorkLocation != null)
            .GroupBy(x => x.WorkLocation!.NameFa)
            .Select(x => new { Label = x.Key, Value = x.Count() })
            .OrderByDescending(x => x.Value).Take(8).ToListAsync(cancellationToken);
        var workLocations = workLocationRows
            .Select(x => new DashboardMetricDto(x.Label, x.Value))
            .ToArray();

        var assets = await dbContext.Assets.AsNoTracking()
            .GroupBy(x => x.Status)
            .Select(x => new { Status = x.Key, Count = x.Count() })
            .ToListAsync(cancellationToken);
        var assetMetrics = assets.Select(x => new DashboardMetricDto(AssetStatusName(x.Status), x.Count)).ToArray();

        var recentEmployees = await dbContext.Employees.AsNoTracking()
            .OrderByDescending(x => x.ModifiedAt ?? x.CreatedAt)
            .Take(8)
            .Select(x => new RecentEmployeeDto(
                x.Id,
                x.EmployeeNumber,
                x.PersonalDetails == null ? "بدون نام" : x.PersonalDetails.FirstNameFa + " " + x.PersonalDetails.LastNameFa,
                x.IsActive,
                x.ModifiedAt ?? x.CreatedAt))
            .ToListAsync(cancellationToken);

        var withoutEmployment = await dbContext.Employees.AsNoTracking()
            .CountAsync(x => x.IsActive && !dbContext.Employments.Any(e => e.EmployeeId == x.Id && e.EndDate == null), cancellationToken);
        var withoutAssignment = await dbContext.Employments.AsNoTracking()
            .CountAsync(x => x.EndDate == null && !dbContext.EmployeeAssignments.Any(a => a.EmploymentId == x.Id && a.EffectiveTo == null && a.Context == AssignmentContext.Hr), cancellationToken);
        var lost = assets.FirstOrDefault(x => x.Status == AssetStatus.Lost)?.Count ?? 0;
        var maintenance = assets.FirstOrDefault(x => x.Status == AssetStatus.UnderMaintenance)?.Count ?? 0;

        return new DashboardDto(
            total, active, total - active, newEmployees, employmentStatuses, employmentTypes, organizationUnits,
            workLocations, assetMetrics, recentEmployees, withoutEmployment, withoutAssignment, lost, maintenance);
    }

    private static string AssetStatusName(AssetStatus status) => status switch
    {
        AssetStatus.Available => "آماده واگذاری",
        AssetStatus.Assigned => "واگذارشده",
        AssetStatus.UnderMaintenance => "در تعمیر",
        AssetStatus.Retired => "از رده خارج",
        AssetStatus.Lost => "مفقود",
        _ => status.ToString()
    };
}
