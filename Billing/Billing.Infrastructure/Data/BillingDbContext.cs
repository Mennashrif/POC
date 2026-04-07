using Billing.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Billing.Infrastructure.Data;

public class BillingDbContext : DbContext
{
    public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options)
    {
    }

    public DbSet<Bill> Bills { get; set; }

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
    }
}
