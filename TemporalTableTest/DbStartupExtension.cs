using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TemporalTableTest.Repository;

namespace TemporalTableTest;

public static class DbStartupExtension
{
    public static IServiceCollection RegisterAppDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<AuditableEntitySaveChangesInterceptor>();
        services.AddTransient<ApplicationDbContextInitialiser>();

        var connectionString = configuration.GetConnectionString("ReferralConnection");
        ArgumentException.ThrowIfNullOrEmpty(connectionString);

        var connection = new SqlConnectionStringBuilder(connectionString).ToString();

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connection, mg =>
                    mg.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.ToString()));
        });

        return services;
    }

}