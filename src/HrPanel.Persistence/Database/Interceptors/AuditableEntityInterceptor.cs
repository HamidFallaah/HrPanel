using HrPanel.Application.Common.Abstractions.Services;
using HrPanel.Persistence.Auditing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HrPanel.Persistence.Database.Interceptors;

public sealed class AuditableEntityInterceptor: SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AuditableEntityInterceptor(ICurrentUserService currentUserService,IDateTimeProvider dateTimeProvider)
    {
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData,InterceptionResult<int> result)
    {
        ApplyAuditValues(eventData.Context);

        return base.SavingChanges(eventData,result);
    }
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,InterceptionResult<int> result,CancellationToken cancellationToken = default)
    {
        ApplyAuditValues(eventData.Context);

        return base.SavingChangesAsync(eventData,result,cancellationToken);
    }
    private void ApplyAuditValues(DbContext? dbContext)
    {
        if (dbContext is null)
        {
            return;
        }

        var now = _dateTimeProvider.Now;
        var userId = _currentUserService.UserId;

        foreach (var entry in dbContext.ChangeTracker.Entries())
        {
            var createdAtProperty = entry.Metadata.FindProperty(AuditPropertyNames.CreatedAt);

            if (createdAtProperty is null)
            {
                continue;
            }

            if (entry.State == EntityState.Added)
            {
                entry.Property(AuditPropertyNames.CreatedAt).CurrentValue = now;
                entry.Property(AuditPropertyNames.CreatedByUserId).CurrentValue = userId;
                entry.Property(AuditPropertyNames.ModifiedAt).CurrentValue = null;
                entry.Property(AuditPropertyNames.ModifiedByUserId).CurrentValue = null;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Property(AuditPropertyNames.CreatedAt).IsModified = false;
                entry.Property(AuditPropertyNames.CreatedByUserId).IsModified = false;
                entry.Property(AuditPropertyNames.ModifiedAt).CurrentValue = now;
                entry.Property(AuditPropertyNames.ModifiedByUserId).CurrentValue = userId;
            }
        }
    }
}
