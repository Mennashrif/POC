using Booking.Application.Abstractions;
using Booking.Application.DTOs;
using Booking.Domain.Abstractions;
using Booking.Domain.Models;
using System.Transactions;

namespace Booking.Application.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _repository;
    private readonly ITransactionRepository _transactionRepository ;
    private const int HotelCapacityPerRoomType = 10;

    public ReservationService(IReservationRepository repository, ITransactionRepository transactionRepository)
    {
        _repository = repository;
        _transactionRepository  = transactionRepository;
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

        using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

        await _repository.AddAsync(reservation);
        await _repository.SaveChangesAsync();

        await SaveOutboxTransactionAsync("ReservationConfirmedEvent", new
        {
            reservation.Id,
            guest.Name,
            guest.Email,
            CheckIn = stayDate.CheckIn,
            CheckOut = stayDate.CheckOut
        });

        scope.Complete();

        return Result<Guid>.Success(reservation.Id);
    }

    public async Task<Result<bool>> UpdateAsync(Guid reservationId, GuestDetails guest, DateTime checkIn, DateTime checkOut)
    {
        var reservation = await _repository.GetByIdAsync(reservationId);
        if (reservation is null)
            return Result<bool>.Failure($"Reservation {reservationId} not found.");

        var stayDate = new StayDate(checkIn, checkOut);
        reservation.Update(guest, stayDate);

        _repository.Update(reservation);
        await _repository.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> CheckInAsync(Guid reservationId, List<string> physicalRoomIds)
    {
        var reservation = await _repository.GetByIdAsync(reservationId);
        if (reservation is null)
            return Result<bool>.Failure($"Reservation {reservationId} not found.");

        reservation.CheckIn(physicalRoomIds);

        using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

        await _repository.SaveChangesAsync();

        await SaveOutboxTransactionAsync("ReservationCheckedInEvent", new
        {
            ReservationId = reservation.Id,
            GuestName = reservation.Guest.Name,
            PhysicalRoomIds = physicalRoomIds,
            CheckInDate = DateTime.UtcNow,
            OccurredAt = DateTime.UtcNow
        });

        scope.Complete();

        return Result<bool>.Success(true);
    }

    public async Task<ReservationDto?> GetByIdAsync(Guid id)
    {
        return await _repository.GetDetailsByIdAsync(id);
    }

    private async Task SaveOutboxTransactionAsync(string eventType, object payload)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(payload);
        var transaction = new Domain.Models.Transaction(eventType, json);
        await _transactionRepository.AddAsync(transaction);
        await _transactionRepository.SaveChangesAsync();
    }
}
