using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TemporalTableTest.Repository;

public class ReferralContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    ApplicationDbContext IDesignTimeDbContextFactory<ApplicationDbContext>.CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = configuration.GetConnectionString("ReferralConnection");

        builder.UseSqlServer(connectionString);

        IDateTime dateTime = new DateTimeService();
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor = new AuditableEntitySaveChangesInterceptor(dateTime);

        return new ApplicationDbContext(builder.Options, auditableEntitySaveChangesInterceptor);
    }
}
