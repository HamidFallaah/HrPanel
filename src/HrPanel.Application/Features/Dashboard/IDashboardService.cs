using HrPanel.Application.Dtos.Dashboard;

namespace HrPanel.Application.Features.Dashboard;
public interface IDashboardService
{
    Task<DashboardDto> GetAsync(CancellationToken cancellationToken = default);
}
