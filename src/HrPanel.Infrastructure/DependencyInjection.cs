using HrPanel.Application.Common.Abstractions.Services;
using HrPanel.Infrastructure.Time;
using Microsoft.Extensions.DependencyInjection;

namespace HrPanel.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider,SystemDateTimeProvider>();

        return services;
    }
}