using System.Text.Json;
using RoomManagement.Application.Abstractions;
using RoomManagement.Domain.Abstractions;
using RoomManagement.Domain.Models;

namespace RoomManagement.Application.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IOutboxRepository _outboxRepository;
    private readonly IProcessedEventRepository _processedEventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RoomService(
        IRoomRepository roomRepository,
        IOutboxRepository outboxRepository,
        IProcessedEventRepository processedEventRepository,
        IUnitOfWork unitOfWork)
    {
        _roomRepository = roomRepository;
        _outboxRepository = outboxRepository;
        _processedEventRepository = processedEventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> AddAsync(string roomNumber, Guid roomTypeId)
    {
        var existing = await _roomRepository.GetByRoomNumberAsync(roomNumber);
        if (existing is not null)
            return Result<Guid>.Failure($"Room number '{roomNumber}' already exists.");

        var room = new Room(roomNumber, roomTypeId);

        await _roomRepository.AddAsync(room);
        await StageOutboxMessageAsync("Room.RoomAddedEvent", new
        {
            RoomId = room.Id,
            room.RoomNumber,
            room.RoomTypeId,
            OccurredAt = DateTime.UtcNow
        });

        await _unitOfWork.SaveChangesAsync();

        return Result<Guid>.Success(room.Id);
    }

    public async Task<Result<bool>> UpdateAsync(Guid roomId, string roomNumber, Guid roomTypeId, RoomStatus status)
    {
        var room = await _roomRepository.GetByIdAsync(roomId);
        if (room is null)
            return Result<bool>.Failure($"Room {roomId} not found.");

        var duplicate = await _roomRepository.GetByRoomNumberAsync(roomNumber);
        if (duplicate is not null && duplicate.Id != roomId)
            return Result<bool>.Failure($"Room number '{roomNumber}' is already used by another room.");

        room.Update(roomNumber, roomTypeId);

        switch (status)
        {
            case RoomStatus.Maintenance:
                room.SetMaintenance();
                break;
            case RoomStatus.Available:
                room.SetAvailable();
                break;
            case RoomStatus.Occupied:
                room.SetOccupied();
                break;
        }

        _roomRepository.Update(room);
        await StageOutboxMessageAsync("Room.RoomUpdatedEvent", new
        {
            RoomId = room.Id,
            room.RoomNumber,
            room.RoomTypeId,
            Status = status.ToString(),
            OccurredAt = DateTime.UtcNow
        });

        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task CheckInAsync(Guid reservationId, List<string> physicalRoomNumbers)
    {
        var eventId = reservationId.ToString();
        const string eventType = "ReservationCheckedInEvent";

        var alreadyProcessed = await _processedEventRepository.ExistsAsync(eventId, eventType);
        if (alreadyProcessed)
            return;

        var rooms = await _roomRepository.GetByRoomNumbersAsync(physicalRoomNumbers);

        if (rooms.Count != physicalRoomNumbers.Count)
        {
            await StageOutboxMessageAsync("Room.CheckInFailedEvent", new
            {
                ReservationId = reservationId,
                PhysicalRoomNumbers = physicalRoomNumbers,
                Reason = "One or more rooms not found.",
                OccurredAt = DateTime.UtcNow
            });
            await _unitOfWork.SaveChangesAsync();
            return;
        }

        foreach (var room in rooms)
        {
            if (room.RoomStatus != RoomStatus.Available)
            {
                await StageOutboxMessageAsync("Room.CheckInFailedEvent", new
                {
                    ReservationId = reservationId,
                    PhysicalRoomNumbers = physicalRoomNumbers,
                    Reason = $"Room {room.RoomNumber} is not available for check-in.",
                    OccurredAt = DateTime.UtcNow
                });
                await _unitOfWork.SaveChangesAsync();
                return;
            }
        }

        foreach (var room in rooms)
        {
            room.SetOccupied();
        }
        _roomRepository.UpdateRange(rooms);

        await _processedEventRepository.AddAsync(new ProcessedEvent(eventId, eventType));

        await _unitOfWork.SaveChangesAsync();

        return;
    }

    private async Task StageOutboxMessageAsync(string eventType, object payload)
    {
        var json = JsonSerializer.Serialize(payload);
        await _outboxRepository.AddAsync(new OutboxMessage(eventType, json));
    }
}
