using HrPanel.Application.Common.Abstractions.LegacyImport;
using HrPanel.Application.Common.Abstractions.Persistence;
using HrPanel.Persistence.Database;
using HrPanel.Persistence.Database.Interceptors;
using HrPanel.Persistence.Identity;
using HrPanel.Persistence.LegacyImport;
using HrPanel.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HrPanel.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services,IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");
        }

        AddInterceptors(services);
        AddDatabase(services, connectionString);
        AddRepositories(services);
        AddLegacyImportServices(services);
        AddIdentityServices(services);

        return services;
    }

    private static void AddInterceptors(IServiceCollection services)
    {
        services.AddScoped<AuditableEntityInterceptor>();
    }

    private static void AddDatabase(IServiceCollection services,string connectionString)
    {
        services.AddDbContext<HrDbContext>((serviceProvider, options) =>
            {
                options.UseSqlServer(connectionString,sqlServerOptions =>
                    {
                        var migrationsAssemblyName =typeof(HrDbContext).Assembly.GetName().Name!;

                        sqlServerOptions.MigrationsAssembly( migrationsAssemblyName);

                        sqlServerOptions.MigrationsHistoryTable("__HrPanelMigrationsHistory",DatabaseSchemas.Hr);

                        sqlServerOptions.EnableRetryOnFailure(maxRetryCount: 5,maxRetryDelay: TimeSpan.FromSeconds(10),errorNumbersToAdd: null);
                    });

                options.AddInterceptors(serviceProvider.GetRequiredService<AuditableEntityInterceptor>());
            });
    }
    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IEmployeeRepository,EmployeeRepository>();

        services.AddScoped<IEmploymentRepository,EmploymentRepository>();

        services.AddScoped<IOrganizationRepository,OrganizationRepository>();

        services.AddScoped<ISchedulingRepository,SchedulingRepository>();

        services.AddScoped<IAssetRepository,AssetRepository>();

        services.AddScoped<ILookupRepository,LookupRepository>();
    }
    private static void AddLegacyImportServices(IServiceCollection services)
    {
        services.AddScoped<ILegacyEmployeeImportService,LegacyEmployeeImportService>();

        services.AddScoped<ILegacyOrganizationReferenceImportService,LegacyOrganizationReferenceImportService>();

        services.AddScoped<ILegacyEmploymentImportService,LegacyEmploymentImportService>();

        services.AddScoped<ILegacyRelationshipImportService,LegacyRelationshipImportService>();

        services.AddScoped<ILegacyEducationImportService,LegacyEducationImportService>();

        services.AddScoped<ILegacySchedulingImportService,LegacySchedulingImportService>();

        services.AddScoped<ILegacyOperationalGroupImportService,LegacyOperationalGroupImportService>();
    }

    private static void AddIdentityServices(IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>(
                options =>
                {
                    ConfigureUserOptions(options);
                    ConfigurePasswordOptions(options);
                    ConfigureLockoutOptions(options);
                    ConfigureSignInOptions(options);
                })
            .AddEntityFrameworkStores<HrDbContext>()
            .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>()
            .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.Events.OnRedirectToLogin = context =>HandleCookieRedirect(context,StatusCodes.Status401Unauthorized);

            options.Events.OnRedirectToAccessDenied = context => HandleCookieRedirect(context,StatusCodes.Status403Forbidden);
        });
    }

    private static void ConfigureUserOptions(IdentityOptions options)
    {
        options.User.RequireUniqueEmail = false;
    }

    private static void ConfigurePasswordOptions(
        IdentityOptions options)
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
    }

    private static void ConfigureLockoutOptions(IdentityOptions options)
    {
        options.Lockout.MaxFailedAccessAttempts = 5;

        options.Lockout.DefaultLockoutTimeSpan =TimeSpan.FromMinutes(15);
    }

    private static void ConfigureSignInOptions(IdentityOptions options)
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.SignIn.RequireConfirmedEmail = false;
    }

    private static Task HandleCookieRedirect(RedirectContext<CookieAuthenticationOptions> context,int statusCode)
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = statusCode;

            return Task.CompletedTask;
        }
        context.Response.Redirect(context.RedirectUri);

        return Task.CompletedTask;
    }
}