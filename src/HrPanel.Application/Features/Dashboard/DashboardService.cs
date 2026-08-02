using HrPanel.Application.Common.Abstractions.Persistence;
using HrPanel.Application.Common.Abstractions.Services;
using HrPanel.Application.Dtos.Dashboard;

namespace HrPanel.Application.Features.Dashboard;

internal sealed class DashboardService(IDashboardRepository repository,IDateTimeProvider dateTimeProvider) : IDashboardService
{
    public Task<DashboardDto> GetAsync(CancellationToken cancellationToken = default) => repository.GetAsync(dateTimeProvider.Now.AddDays(-30), cancellationToken);
}
