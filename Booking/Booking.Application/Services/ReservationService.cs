using Booking.Application.Abstractions;
using Booking.Application.DTOs;
using Booking.Domain.Abstractions;
using Booking.Domain.Models;

namespace Booking.Application.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _repository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILocalRoomRepository _localRoomRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReservationService(
        IReservationRepository repository,
        ITransactionRepository transactionRepository,
        ILocalRoomRepository localRoomRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _transactionRepository = transactionRepository;
        _localRoomRepository = localRoomRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> CreateAsync(
        GuestDetails guest,
        DateTime checkIn,
        DateTime checkOut,
        List<RoomRequest> roomRequests)
    {
        var stayDate = new StayDate(checkIn, checkOut);

        var overlapping = await _repository.GetOverlappingAsync(stayDate);

        // Count how many rooms of each type are already booked for these dates
        var takenRooms = new Dictionary<Guid, int>();
        foreach (var res in overlapping)
            foreach (var req in res.RoomRequests)
                takenRooms[req.RoomTypeId] = takenRooms.GetValueOrDefault(req.RoomTypeId) + req.Quantity;

        // Check against actual room inventory from local read model
        foreach (var requested in roomRequests)
        {
            var totalRoomsOfType = await _localRoomRepository.GetTotalCountByRoomTypeIdAsync(requested.RoomTypeId);

            if (totalRoomsOfType == 0)
                return Result<Guid>.Failure($"Room type {requested.RoomTypeId} does not exist.");

            var taken = takenRooms.GetValueOrDefault(requested.RoomTypeId);

            if (taken + requested.Quantity > totalRoomsOfType)
                return Result<Guid>.Failure("The hotel is fully booked for the requested dates and room type!");
        }

        var reservation = new Reservation(guest, stayDate, roomRequests);
        reservation.Confirm();

        _repository.Add(reservation);
        await StageOutboxTransactionAsync("ReservationConfirmedEvent", new
        {
            reservation.Id,
            guest.Name,
            guest.Email,
            CheckIn = stayDate.CheckIn,
            CheckOut = stayDate.CheckOut
        });

        await _unitOfWork.SaveChangesAsync();

        return Result<Guid>.Success(reservation.Id);
    }

    public async Task<Result<bool>> CheckInAsync(Guid reservationId, List<string> physicalRoomIds)
    {
        var reservation = await _repository.GetByIdAsync(reservationId);
        if (reservation is null)
            return Result<bool>.Failure($"Reservation {reservationId} not found.");

        reservation.CheckIn(physicalRoomIds);

        await StageOutboxTransactionAsync("ReservationCheckedInEvent", new
        {
            ReservationId = reservation.Id,
            GuestName = reservation.Guest.Name,
            PhysicalRoomIds = physicalRoomIds,
            CheckInDate = DateTime.UtcNow,
            OccurredAt = DateTime.UtcNow
        });

        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> HandleCheckInFailureAsync(Guid reservationId, string reason)
    {
        var reservation = await _repository.GetByIdAsync(reservationId);
        if (reservation is null)
            return Result<bool>.Failure($"Reservation {reservationId} not found.");


        reservation.RevertCheckIn();

        await StageOutboxTransactionAsync("CheckInRevertedEvent", new
        {
            ReservationId = reservation.Id,
            Reason = reason,
            OccurredAt = DateTime.UtcNow
        });

        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<ReservationDto?> GetByIdAsync(Guid id)
    {
        return await _repository.GetDetailsByIdAsync(id);
    }

    private async Task StageOutboxTransactionAsync(string eventType, object payload)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(payload);
        var transaction = new Domain.Models.Transaction(eventType, json);
        await _transactionRepository.AddAsync(transaction);
    }
}
