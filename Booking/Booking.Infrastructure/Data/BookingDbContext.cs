using Booking.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Data;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
    {
    }

    // This specifically registers the Reservation Aggregate Root so EF Core maps it!
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<LocalRoom> LocalRooms { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.RowVersion)
            .IsRowVersion();

            entity.OwnsOne(r => r.StayDate);

            entity.OwnsOne(r => r.Guest);

            entity.OwnsMany(r => r.RoomRequests);
            entity.Navigation(r => r.RoomRequests)
                .HasField("_roomRequests")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            entity.PrimitiveCollection<List<string>>("_assignedPhysicalRoomIds")
                .HasField("_assignedPhysicalRoomIds")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

        });

        modelBuilder.Entity<LocalRoom>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.RoomNumber).IsRequired().HasMaxLength(20);
            entity.Property(r => r.Status).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.EventType).IsRequired().HasMaxLength(200);
            entity.Property(t => t.Payload).IsRequired();
            entity.Property(t => t.OccurredAt).IsRequired();
            entity.Property(t => t.PublishedAt).IsRequired(false);
        });
    }
}
