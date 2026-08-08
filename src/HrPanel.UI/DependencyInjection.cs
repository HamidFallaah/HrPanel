using HrPanel.Application.Common.Abstractions.Services;
using HrPanel.Application.Common.Authorization;
using HrPanel.UI.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrPanel.UI;

public static class DependencyInjection
{
    private const string DefaultCulture = "fa-IR";

    private static readonly string[] SupportedCultures =
    [
        DefaultCulture,
        "en-US"
    ];

    public static IServiceCollection AddUi(this IServiceCollection services)
    {
        AddCurrentUserServices(services);
        AddLocalizationServices(services);
        AddAuthorizationPolicies(services);
        AddMvcServices(services);

        return services;
    }

    private static void AddCurrentUserServices(IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUserService,CurrentUserService>();
    }

    private static void AddLocalizationServices(IServiceCollection services)
    {
        services.AddLocalization(options =>
        {
            options.ResourcesPath = "Resources";
        });

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options
                .SetDefaultCulture(DefaultCulture)
                .AddSupportedCultures(SupportedCultures)
                .AddSupportedUICultures(SupportedCultures);
        });
    }

    private static void AddAuthorizationPolicies(IServiceCollection services)
    {
        services.AddAuthorization(ConfigurePolicies);
    }

    private static void ConfigurePolicies(AuthorizationOptions options)
    {
        options.AddPolicy( PolicyNames.AdministratorOnly,
            policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(RoleNames.Administrator);
            });

        options.AddPolicy(PolicyNames.HrAccess,
            policy =>
            {
                policy.RequireAuthenticatedUser();

                policy.RequireRole(RoleNames.Administrator,RoleNames.HrStaff);
            });
    }

    private static void AddMvcServices(IServiceCollection services)
    {
        services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-TOKEN";
            options.Cookie.Name ="__Host-HrPanel.Antiforgery";

            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

            options.Cookie.Path = "/";
            options.SuppressXFrameOptionsHeader = false;
        });

        services
            .AddControllersWithViews()
            .AddViewLocalization()
            .AddDataAnnotationsLocalization();

        //services.AddEndpointsApiExplorer();
        //services.AddSwaggerGen();
    }
}
