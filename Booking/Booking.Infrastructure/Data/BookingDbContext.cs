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

            entity.Ignore(r => r.DomainEvents);
        });
        
    }
}
