using HrPanel.Application.Dtos.Dashboard;

namespace HrPanel.Application.Common.Abstractions.Persistence;

public interface IDashboardRepository
{
    Task<DashboardDto> GetAsync(DateTime recentFrom, CancellationToken cancellationToken = default);
}
