using HrPanel.Persistence.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace HrPanel.Host;

public static class IdentitySeedExtensions
{
    public static async Task SeedIdentityAsync(this IServiceProvider serviceProvider,CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        await using var scope = serviceProvider.CreateAsyncScope();

        var identitySeeder = scope.ServiceProvider.GetRequiredService<IIdentitySeeder>();

        await identitySeeder.SeedAsync(cancellationToken);
    }
}