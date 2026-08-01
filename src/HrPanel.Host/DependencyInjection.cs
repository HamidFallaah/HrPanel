using HrPanel.Application;
using HrPanel.Infrastructure;
using HrPanel.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HrPanel.Host;

public static class DependencyInjection
{
    public static IServiceCollection AddHrPanelModules(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddApplication().AddPersistence(configuration).AddInfrastructure();

        return services;
    }
}
