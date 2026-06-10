// CloudStaff.DataContext/CloudStaffContext.cs
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
