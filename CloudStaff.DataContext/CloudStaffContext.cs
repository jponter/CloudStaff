using CloudStaff.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace CloudStaff.DataContext;

public class CloudStaffContext : DbContext
{
    public CloudStaffContext(DbContextOptions<CloudStaffContext> options) : base(options) { }

    public DbSet<Manager> Managers => Set<Manager>();
    public DbSet<Staff> Staff => Set<Staff>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<ClientProject> ClientProjects => Set<ClientProject>();
    public DbSet<StaffCategory> StaffCategories => Set<StaffCategory>();
    public DbSet<HomePool> HomePools => Set<HomePool>();
    public DbSet<StaffRole> StaffRoles => Set<StaffRole>();
    public DbSet<Platform> Platforms => Set<Platform>();
    public DbSet<StaffPlatform> StaffPlatforms => Set<StaffPlatform>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Manager>(e =>
        {
            e.ToTable("managers");
            e.HasKey(m => m.Id);
            e.Property(m => m.Name).IsRequired().HasMaxLength(200);
            e.Property(m => m.AsNumber).IsRequired().HasMaxLength(20);
            e.HasIndex(m => m.AsNumber).IsUnique();
            e.Property(m => m.Location).IsRequired().HasMaxLength(200);
            e.Property(m => m.Email).IsRequired().HasMaxLength(200);
            e.HasMany(m => m.Staff)
             .WithOne(s => s.Manager)
             .HasForeignKey(s => s.ManagerId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Staff>(e =>
        {
            e.ToTable("staff");
            e.HasKey(s => s.Id);
            e.Property(s => s.Name).IsRequired().HasMaxLength(200);
            e.Property(s => s.AsNumber).IsRequired().HasMaxLength(20);
            e.HasIndex(s => s.AsNumber).IsUnique();
            e.Property(s => s.ContractJobTitle).HasMaxLength(200);
            e.HasOne(s => s.HomePool)
             .WithMany(h => h.Staff)
             .HasForeignKey(s => s.HomePoolId)
             .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(s => s.StaffRole)
             .WithMany(r => r.Staff)
             .HasForeignKey(s => s.StaffRoleId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<StaffCategory>(e =>
        {
            e.ToTable("staff_categories");
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<HomePool>(e =>
        {
            e.ToTable("home_pools");
            e.HasKey(h => h.Id);
            e.Property(h => h.Code).IsRequired().HasMaxLength(10);
            e.HasIndex(h => h.Code).IsUnique();
            e.Property(h => h.Name).IsRequired().HasMaxLength(200);
            e.HasOne(h => h.Category)
             .WithMany(c => c.HomePools)
             .HasForeignKey(h => h.CategoryId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<StaffRole>(e =>
        {
            e.ToTable("staff_roles");
            e.HasKey(r => r.Id);
            e.Property(r => r.Title).IsRequired().HasMaxLength(200);
            e.HasOne(r => r.HomePool)
             .WithMany(h => h.Roles)
             .HasForeignKey(r => r.HomePoolId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Platform>(e =>
        {
            e.ToTable("platforms");
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).IsRequired().HasMaxLength(100);
            e.HasIndex(p => p.Name).IsUnique();
        });

        modelBuilder.Entity<StaffPlatform>(e =>
        {
            e.ToTable("staff_platforms");
            e.HasKey(sp => new { sp.StaffId, sp.PlatformId });
            e.HasOne(sp => sp.Staff)
             .WithMany(s => s.StaffPlatforms)
             .HasForeignKey(sp => sp.StaffId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(sp => sp.Platform)
             .WithMany(p => p.StaffPlatforms)
             .HasForeignKey(sp => sp.PlatformId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Client>(e =>
        {
            e.ToTable("clients");
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).IsRequired().HasMaxLength(200);
            e.Property(c => c.Description).HasMaxLength(1000);
            e.HasMany(c => c.Projects)
             .WithOne(p => p.Client)
             .HasForeignKey(p => p.ClientId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ClientProject>(e =>
        {
            e.ToTable("client_projects");
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).IsRequired().HasMaxLength(200);
            e.Property(p => p.Description).HasMaxLength(1000);
        });
    }
}