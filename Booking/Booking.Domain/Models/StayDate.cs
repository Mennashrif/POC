using System;

namespace Booking.Domain.Models;

public record StayDate
{
    public DateTime CheckIn { get; init; }
    public DateTime CheckOut { get; init; }

    public int TotalNights => (CheckOut.Date - CheckIn.Date).Days;

    public StayDate(DateTime checkIn, DateTime checkOut)
    {
        if (checkOut.Date <= checkIn.Date)
            throw new ArgumentException("Check-out date must be after Check-in date.");
            
        CheckIn = checkIn.Date;
        CheckOut = checkOut.Date;
    }
}
