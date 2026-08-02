using FluentValidation;
using HrPanel.Application.Features.Employees;
using HrPanel.Application.Features.Assets;
using HrPanel.Application.Features.Employments;
using HrPanel.Application.Features.LegacyImport;
using HrPanel.Application.Features.Lookups;
using HrPanel.Application.Features.Organization;
using HrPanel.Application.Features.Scheduling;
using Microsoft.Extensions.DependencyInjection;
using HrPanel.Application.Features.Dashboard;

namespace HrPanel.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddValidatorsFromAssembly(assembly);
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddSingleton<IEmployeeLookupService, EmployeeLookupService>();
        services.AddScoped<ILookupService, LookupService>();
        services.AddScoped<IEmploymentService, EmploymentService>();
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<ISchedulingService, SchedulingService>();
        services.AddScoped<IAssetService, AssetService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ILegacyImportService, LegacyImportService>();

        return services;
    }
}
