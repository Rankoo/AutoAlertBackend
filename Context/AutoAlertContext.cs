using Microsoft.EntityFrameworkCore;
using AutoAlertBackEnd.Models;

namespace AutoAlertBackEnd.Context
{
    public class AutoAlertContext : DbContext
    {
        public AutoAlertContext(DbContextOptions<AutoAlertContext> options) : base(options) { }

        public DbSet<Groups> Groups { get; set; } = null!;
        public DbSet<Companies> Companies { get; set; } = null!;
        public DbSet<Stores> Stores { get; set; } = null!;
        public DbSet<DocumentTypes> DocumentTypes { get; set; } = null!;
        public DbSet<Roles> Roles { get; set; } = null!;
        public DbSet<Users> Users { get; set; } = null!;
        public DbSet<Modules> Modules { get; set; } = null!;
        public DbSet<SubModules> SubModules { get; set; } = null!;
        public DbSet<RoleSubModules> RoleSubModules { get; set; } = null!;
        public DbSet<UserSubmodules> UserSubmodules { get; set; } = null!;
        public DbSet<Services> Services { get; set; } = null!;
        public DbSet<Alerts> Alerts { get; set; } = null!;
        public DbSet<Notifications> Notifications { get; set; } = null!;
        public DbSet<UserCompanies> UserCompanies { get; set; } = null!;
        public DbSet<UserGroups> UserGroups { get; set; } = null!;
        public DbSet<Logs> Logs { get; set; } = null!;

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<DateTime>().HaveColumnType("timestamp without time zone");
            configurationBuilder.Properties<DateTime?>().HaveColumnType("timestamp without time zone");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Groups
            modelBuilder.Entity<Groups>(e =>
            {
                e.ToTable("Groups");
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired().HasMaxLength(100);
                e.Property(x => x.Description).HasMaxLength(255);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");
            });

            // Companies
            modelBuilder.Entity<Companies>(e =>
            {
                e.ToTable("Companies");
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired().HasMaxLength(100);
                e.Property(x => x.NIT).HasMaxLength(20);
                e.Property(x => x.Address).HasMaxLength(150);
                e.Property(x => x.PhoneNumber).HasMaxLength(20);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");

                e.HasOne(x => x.Group)
                 .WithMany(g => g.Companies)
                 .HasForeignKey(x => x.GroupId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // Stores
            modelBuilder.Entity<Stores>(e =>
            {
                e.ToTable("Stores");
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired().HasMaxLength(100);
                e.Property(x => x.Address).HasMaxLength(150);
                e.Property(x => x.City).HasMaxLength(100);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");

                e.HasOne(x => x.Company)
                 .WithMany(c => c.Stores)
                 .HasForeignKey(x => x.CompanyId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // DocumentTypes
            modelBuilder.Entity<DocumentTypes>(e =>
            {
                e.ToTable("DocumentTypes");
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired().HasMaxLength(50);
                e.Property(x => x.Abbreviation).HasMaxLength(10);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");
            });

            // Roles
            modelBuilder.Entity<Roles>(e =>
            {
                e.ToTable("Roles");
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired().HasMaxLength(100);
                e.Property(x => x.Description).HasMaxLength(255);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");
            });

            // Users
            modelBuilder.Entity<Users>(e =>
            {
                e.ToTable("Users");
                e.HasKey(x => x.Id);
                e.Property(x => x.Names).IsRequired().HasMaxLength(150);
                e.Property(x => x.LastNames).HasMaxLength(150);
                e.Property(x => x.Email).IsRequired().HasMaxLength(150);
                e.HasIndex(x => x.Email).IsUnique();
                e.Property(x => x.PasswordHash).HasMaxLength(255);
                e.Property(x => x.Address).HasMaxLength(255);
                e.Property(x => x.PhoneNumber).HasMaxLength(20);
                e.Property(x => x.GoogleId).HasMaxLength(255);
                e.Property(x => x.DocumentNumber).HasMaxLength(50);
                e.Property(x => x.Position).HasMaxLength(100);
                e.Property(x => x.IsActive).HasDefaultValue(true);
                e.Property(x => x.ChangePassword).HasDefaultValue(true);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");

                e.HasOne(x => x.Role)
                 .WithMany(r => r.Users)
                 .HasForeignKey(x => x.RoleId)
                 .OnDelete(DeleteBehavior.SetNull);

                e.HasOne(x => x.DocumentType)
                 .WithMany(d => d.Users)
                 .HasForeignKey(x => x.DocumentTypeId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // Modules
            modelBuilder.Entity<Modules>(e =>
            {
                e.ToTable("Modules");
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired().HasMaxLength(100);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");
            });

            // SubModules
            modelBuilder.Entity<SubModules>(e =>
            {
                e.ToTable("SubModules");
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired().HasMaxLength(100);

                e.HasOne(x => x.Module)
                 .WithMany(m => m.SubModules)
                 .HasForeignKey(x => x.ModuleId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // RoleSubModules
            modelBuilder.Entity<RoleSubModules>(e =>
            {
                e.ToTable("RoleSubModules");
                e.HasKey(x => x.Id);
                e.Property(x => x.IsEnabled).HasDefaultValue(false);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");

                e.HasOne(x => x.Role)
                 .WithMany(r => r.RoleSubModules)
                 .HasForeignKey(x => x.RoleId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.SubModule)
                 .WithMany(s => s.RoleSubModules)
                 .HasForeignKey(x => x.SubModuleId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // UserSubmodules (composite PK)
            modelBuilder.Entity<UserSubmodules>(e =>
            {
                e.ToTable("UserSubmodules");
                e.HasKey(x => new { x.UserId, x.SubModuleId });
                e.Property(x => x.IsEnabled).HasDefaultValue(false);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");

                e.HasOne(x => x.User)
                 .WithMany(u => u.UserSubmodules)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.SubModule)
                 .WithMany(s => s.UserSubmodules)
                 .HasForeignKey(x => x.SubModuleId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // Services
            modelBuilder.Entity<Services>(e =>
            {
                e.ToTable("Services");
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired().HasMaxLength(150);
                e.Property(x => x.Provider).HasMaxLength(100);
                e.Property(x => x.AccountNumber).HasMaxLength(100);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");

                e.HasOne(x => x.Store)
                 .WithMany(s => s.Services)
                 .HasForeignKey(x => x.StoreId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // Alerts
            modelBuilder.Entity<Alerts>(e =>
            {
                e.ToTable("Alerts");
                e.HasKey(x => x.Id);
                e.Property(x => x.DueDate).HasColumnType("date").IsRequired();
                e.Property(x => x.Amount).HasColumnType("numeric(18,2)").IsRequired();
                e.Property(x => x.Status).HasMaxLength(50).HasDefaultValue("Pendiente");
                e.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");

                e.HasOne(x => x.Service)
                 .WithMany(s => s.Alerts)
                 .HasForeignKey(x => x.ServiceId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // Notifications
            modelBuilder.Entity<Notifications>(e =>
            {
                e.ToTable("Notifications");
                e.HasKey(x => x.Id);
                e.Property(x => x.Result).HasMaxLength(100);
                e.Property(x => x.Channel).HasMaxLength(50);
                e.Property(x => x.Title).HasMaxLength(150);
                e.Property(x => x.Message).HasMaxLength(500);
                e.Property(x => x.IsRead).HasDefaultValue(false);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");
                e.Property(x => x.AlertId).IsRequired();
                e.Property(x => x.UserId).IsRequired();

                e.HasOne(x => x.Alert)
                 .WithMany(a => a.Notifications)
                 .HasForeignKey(x => x.AlertId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.User)
                 .WithMany(u => u.Notifications)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // UserCompanies (composite PK)
            modelBuilder.Entity<UserCompanies>(e =>
            {
                e.ToTable("UserCompanies");
                e.HasKey(x => new { x.UserId, x.CompanyId });
                e.Property(x => x.AccessType).HasMaxLength(50).HasDefaultValue("view");
                e.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");

                e.HasOne(x => x.User)
                 .WithMany(u => u.UserCompanies)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Company)
                 .WithMany(c => c.UserCompanies)
                 .HasForeignKey(x => x.CompanyId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // UserGroups (composite PK)
            modelBuilder.Entity<UserGroups>(e =>
            {
                e.ToTable("UserGroups");
                e.HasKey(x => new { x.UserId, x.GroupId });
                e.Property(x => x.AccessType).HasMaxLength(50).HasDefaultValue("view");
                e.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()");

                e.HasOne(x => x.User)
                 .WithMany(u => u.UserGroups)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Group)
                 .WithMany(g => g.UserGroups)
                 .HasForeignKey(x => x.GroupId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // Logs
            modelBuilder.Entity<Logs>(e =>
            {
                e.ToTable("Logs");
                e.HasKey(x => x.Id);
                e.Property(x => x.Action).IsRequired().HasMaxLength(100);
                e.Property(x => x.TableName).IsRequired().HasMaxLength(100);
                e.Property(x => x.OldValues).HasColumnType("text");
                e.Property(x => x.NewValues).HasColumnType("text");
                e.Property(x => x.Timestamp).HasDefaultValueSql("NOW()");
                e.Property(x => x.IpAddress).HasMaxLength(50);

                e.HasOne(x => x.User)
                 .WithMany() // no navigation property on Users for Logs
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
