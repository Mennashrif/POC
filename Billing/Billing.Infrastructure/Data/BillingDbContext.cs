using Billing.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Billing.Infrastructure.Data;

public class BillingDbContext : DbContext
{
    public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options)
    {
    }

    public DbSet<Bill> Bills { get; set; }
    public DbSet<ProcessedEvent> ProcessedEvents { get; set; }

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
    }
}
