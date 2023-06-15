using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TemporalTableTest.Entities;

namespace TemporalTableTest.Repository;

public class ApplicationDbContext : DbContext
{
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;
    public ApplicationDbContext
        (
            DbContextOptions<ApplicationDbContext> options,
            AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor
        )
        : base(options)
    {
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<Entities.ReferralService>().Property(c => c.Id).ValueGeneratedNever();
        modelBuilder.Entity<Entities.ReferralOrganisation>().Property(c => c.Id).ValueGeneratedNever();

        modelBuilder
         .Entity<ReferralStatus>()
         .ToTable("ReferralStatuses", b => b.IsTemporal());

        modelBuilder
        .Entity<Recipient>()
        .ToTable("Recipients", b => b.IsTemporal());

        modelBuilder
        .Entity<Entities.ReferralService>()
        .ToTable("ReferralServices", b => b.IsTemporal());

        modelBuilder
        .Entity<ReferralOrganisation>()
        .ToTable("ReferralOrganisations", b => b.IsTemporal());


        modelBuilder
        .Entity<Entities.Referral>()
        .ToTable("Referrals", b => b.IsTemporal());

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }

    public DbSet<Entities.Referral> Referrals => Set<Entities.Referral>();
    public DbSet<Recipient> Recipients => Set<Recipient>();
    public DbSet<ReferralOrganisation> ReferralOrganisations => Set<ReferralOrganisation>();
    public DbSet<Entities.ReferralService> ReferralServices => Set<Entities.ReferralService>();
    public DbSet<ReferralStatus> ReferralStatuses => Set<ReferralStatus>();
    public DbSet<Referrer> Referrers => Set<Referrer>();
}

