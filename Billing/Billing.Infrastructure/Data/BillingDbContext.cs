using Billing.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Billing.Infrastructure.Data;

public class BillingDbContext : DbContext
{
    public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options)
    {
    }

    public DbSet<Bill> Bills { get; set; }
    public DbSet<ProcessedEvent> ProcessedEvents { get; set; }
    public DbSet<BillingFile> BillingFiles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(b => b.Id);

            entity.PrimitiveCollection<List<string>>("_physicalRoomIds")
                .HasField("_physicalRoomIds")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            entity.Ignore(b => b.DomainEvents);
        });

        modelBuilder.Entity<ProcessedEvent>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.HasIndex(p => new { p.ReservationId, p.EventType }).IsUnique();
            entity.Property(p => p.EventType).IsRequired().HasMaxLength(200);
            entity.Property(p => p.ProcessedAt).IsRequired();
            entity.Ignore(p => p.DomainEvents);
        });

        modelBuilder.Entity<BillingFile>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.Property(f => f.FileName).IsRequired().HasMaxLength(500);
            entity.Property(f => f.ContentType).IsRequired().HasMaxLength(100);
            entity.Property(f => f.Hash).IsRequired().HasMaxLength(64);
            entity.Property(f => f.StoragePath).IsRequired().HasMaxLength(1000);
            entity.Property(f => f.UploadedAt).IsRequired();
            entity.Ignore(f => f.DomainEvents);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(rp => rp.Id);
            entity.Property(rp => rp.Role).IsRequired().HasMaxLength(100);
            entity.Property(rp => rp.Permission).IsRequired().HasMaxLength(100);
            entity.HasIndex(rp => new { rp.Role, rp.Permission }).IsUnique();
            entity.Ignore(rp => rp.DomainEvents);
        });

        // Seed data
        modelBuilder.Entity<RolePermission>().HasData(
            new { Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000001"), Role = "Admin", Permission = "read:bills" },
            new { Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000002"), Role = "Admin", Permission = "write:bills" },
            new { Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000003"), Role = "Admin", Permission = "upload:files" },
            new { Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000004"), Role = "Admin", Permission = "download:files" },
            new { Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000005"), Role = "User",  Permission = "read:bills" },
            new { Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000006"), Role = "User",  Permission = "download:files" }
        );
    }
}
