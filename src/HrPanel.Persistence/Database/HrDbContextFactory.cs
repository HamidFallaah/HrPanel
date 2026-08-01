using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HrPanel.Persistence.Database;

public sealed class HrDbContextFactory: IDesignTimeDbContextFactory<HrDbContext>
{
    public HrDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HrDbContext>();

        var connectionString = Environment.GetEnvironmentVariable("HRPANEL_CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString ="Server=t1vwvas034;" +"Database=Hamid;" +"Integrated Security=True;" +"MultipleActiveResultSets=True;" +"Encrypt=True;" +"TrustServerCertificate=True;";
        }

        optionsBuilder.UseSqlServer(connectionString,
            sqlServerOptions =>
            {
                sqlServerOptions.MigrationsAssembly(typeof(HrDbContext).Assembly.GetName().Name);

                sqlServerOptions.MigrationsHistoryTable("__HrPanelMigrationsHistory", DatabaseSchemas.Hr);

                sqlServerOptions.EnableRetryOnFailure(maxRetryCount: 5,maxRetryDelay: TimeSpan.FromSeconds(10),errorNumbersToAdd: null);
            });

        return new HrDbContext(optionsBuilder.Options);
    }
}