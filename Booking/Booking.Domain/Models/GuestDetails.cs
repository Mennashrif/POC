namespace Booking.Domain.Models;

// This is a Value Object (V)
// It has no Identity (ID). It is purely defined by its data.
public record GuestDetails(string Name, string Email, string Phone)
{
    // Records automatically provide value-based equality checking.
    // If two guests have the same name, email, and phone, they are considered equal.
}
