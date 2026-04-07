using Booking.Application.Abstractions;
using Booking.Application.DTOs;
using Booking.Domain.Abstractions;
using Booking.Domain.Models;

namespace Booking.Application.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _repository;

    private const int HotelCapacityPerRoomType = 10;

    public ReservationService(IReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Guid>> CreateAsync(
        GuestDetails guest,
        DateTime checkIn,
        DateTime checkOut,
        List<RoomRequest> roomRequests)
    {
        var stayDate = new StayDate(checkIn, checkOut);

        var overlapping = await _repository.GetOverlappingAsync(stayDate);

        var takenRooms = new Dictionary<RoomTypeEnum, int>();
        foreach (var res in overlapping)
            foreach (var req in res.RoomRequests)
                takenRooms[req.RoomType] = takenRooms.GetValueOrDefault(req.RoomType) + req.Quantity;

        foreach (var requested in roomRequests)
        {
            if (takenRooms.GetValueOrDefault(requested.RoomType) + requested.Quantity > HotelCapacityPerRoomType)
                return Result<Guid>.Failure("The hotel is fully booked for the requested dates and room type!");
        }

        var reservation = new Reservation(guest, stayDate, roomRequests);
        reservation.Confirm();

        await _repository.AddAsync(reservation);
        await _repository.SaveChangesAsync();

        return Result<Guid>.Success(reservation.Id);
    }

    public async Task<Result<bool>> CheckInAsync(Guid reservationId, List<string> physicalRoomIds)
    {
        var reservation = await _repository.GetByIdAsync(reservationId);
        if (reservation is null)
            return Result<bool>.Failure($"Reservation {reservationId} not found.");

        reservation.CheckIn(physicalRoomIds);
        await _repository.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<ReservationDto?> GetByIdAsync(Guid id)
    {
        return await _repository.GetDetailsByIdAsync(id);
    }
}
