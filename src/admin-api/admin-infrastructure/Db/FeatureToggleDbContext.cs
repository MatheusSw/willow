using admin_infrastructure.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace admin_infrastructure.Db;

public class FeatureToggleDbContext : DbContext
{
    public FeatureToggleDbContext(DbContextOptions<FeatureToggleDbContext> options) : base(options)
    {
    }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<EnvironmentEntity> Environments => Set<EnvironmentEntity>();
    public DbSet<Feature> Features => Set<Feature>();
    public DbSet<FeatureState> FeatureStates => Set<FeatureState>();
    public DbSet<Rule> Rules => Set<Rule>();
    public DbSet<ApiKey> ApiKeys => Set<ApiKey>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<Organization>(e =>
        {
            e.ToTable("organizations");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Name).IsRequired().HasColumnName("name");
            e.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<Project>(e =>
        {
            e.ToTable("projects");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.OrgId).HasColumnName("org_id");
            e.Property(x => x.Name).IsRequired().HasColumnName("name");
            e.HasIndex(x => new { x.OrgId, x.Name }).IsUnique();
            e.HasOne(x => x.Organization)
                .WithMany()
                .HasForeignKey(x => x.OrgId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EnvironmentEntity>(e =>
        {
            e.ToTable("environments");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.ProjectId).HasColumnName("project_id");
            e.Property(x => x.Key).IsRequired().HasColumnName("key");
            e.HasIndex(x => new { x.ProjectId, x.Key }).IsUnique();
            e.HasOne(x => x.Project)
                .WithMany()
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Feature>(e =>
        {
            e.ToTable("features");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.ProjectId).HasColumnName("project_id");
            e.Property(x => x.Name).IsRequired().HasColumnName("name");
            e.Property(x => x.Description).HasColumnName("description");
            e.HasIndex(x => x.ProjectId);
            e.HasIndex(x => new { x.ProjectId, x.Name }).IsUnique();
            e.HasOne(x => x.Project)
                .WithMany()
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FeatureState>(e =>
        {
            e.ToTable("feature_states");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.FeatureId).HasColumnName("feature_id");
            e.Property(x => x.EnvironmentId).HasColumnName("environment_id");
            e.Property(x => x.Enabled).HasColumnName("enabled").HasDefaultValue(false);
            e.Property(x => x.Reason).HasColumnName("reason");
            e.HasIndex(x => new { x.FeatureId, x.EnvironmentId }).IsUnique();
            e.HasOne(x => x.Feature)
                .WithMany()
                .HasForeignKey(x => x.FeatureId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Environment)
                .WithMany()
                .HasForeignKey(x => x.EnvironmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Rule>(e =>
        {
            e.ToTable("rules");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.FeatureId).HasColumnName("feature_id");
            e.Property(x => x.EnvironmentId).HasColumnName("environment_id");
            e.Property(x => x.Priority).HasColumnName("priority");
            e.Property(x => x.MatchType).HasColumnName("match_type");
            e.Property(x => x.ConditionsJson).HasColumnName("conditions").HasColumnType("jsonb");
            e.HasCheckConstraint("CK_rules_match_type", "match_type IN ('all','any')");
            e.HasIndex(x => new { x.FeatureId, x.EnvironmentId });
            e.HasOne(x => x.Feature)
                .WithMany()
                .HasForeignKey(x => x.FeatureId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Environment)
                .WithMany()
                .HasForeignKey(x => x.EnvironmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ApiKey>(e =>
        {
            e.ToTable("api_keys");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.OrgId).HasColumnName("org_id");
            e.Property(x => x.ProjectId).HasColumnName("project_id");
            e.Property(x => x.Role).HasColumnName("role");
            e.Property(x => x.Scopes).HasColumnName("scopes").HasColumnType("text[]");
            e.Property(x => x.HashedKey).HasColumnName("hashed_key");
            e.Property(x => x.Active).HasColumnName("active").HasDefaultValue(true);
            e.HasIndex(x => new { x.OrgId, x.ProjectId });
            e.HasOne(x => x.Organization)
                .WithMany()
                .HasForeignKey(x => x.OrgId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Project)
                .WithMany()
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AuditLog>(e =>
        {
            e.ToTable("audit_logs");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Actor).HasColumnName("actor");
            e.Property(x => x.Action).HasColumnName("action");
            e.Property(x => x.EntityType).HasColumnName("entity_type");
            e.Property(x => x.EntityId).HasColumnName("entity_id");
            e.Property(x => x.Before).HasColumnName("before").HasColumnType("jsonb");
            e.Property(x => x.After).HasColumnName("after").HasColumnType("jsonb");
            e.Property(x => x.At).HasColumnName("at").HasDefaultValueSql("now()");
            e.HasIndex(x => new { x.EntityType, x.EntityId });
        });
    }
}


