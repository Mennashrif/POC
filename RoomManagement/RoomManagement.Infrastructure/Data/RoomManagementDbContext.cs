using Microsoft.EntityFrameworkCore;
using RoomManagement.Domain.Models;

namespace RoomManagement.Infrastructure.Data;

public class RoomManagementDbContext : DbContext
{
    public RoomManagementDbContext(DbContextOptions<RoomManagementDbContext> options) : base(options) { }

    public DbSet<Room> Rooms { get; set; }
    public DbSet<RoomType> RoomTypes { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<ProcessedEvent> ProcessedEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.RoomNumber).IsRequired().HasMaxLength(20);
            entity.HasIndex(r => r.RoomNumber).IsUnique();
            entity.HasOne(r => r.RoomType)
                  .WithMany()
                  .HasForeignKey(r => r.RoomTypeId);
        });

        modelBuilder.Entity<RoomType>(entity =>
        {
            entity.HasKey(rt => rt.Id);
            entity.Property(rt => rt.Name).IsRequired().HasMaxLength(100);
            entity.Property(rt => rt.Price).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.EventType).IsRequired().HasMaxLength(200);
            entity.Property(o => o.Payload).IsRequired();
            entity.Property(o => o.OccurredAt).IsRequired();
            entity.Property(o => o.PublishedAt).IsRequired(false);
        });

        modelBuilder.Entity<ProcessedEvent>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.HasIndex(p => new { p.EventId, p.EventType }).IsUnique();
            entity.Property(p => p.EventId).IsRequired().HasMaxLength(100);
            entity.Property(p => p.EventType).IsRequired().HasMaxLength(200);
        });
    }
}
