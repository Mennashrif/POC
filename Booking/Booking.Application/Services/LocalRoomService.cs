using Booking.Application.Abstractions;
using Booking.Domain.Abstractions;
using Booking.Domain.Models;

namespace Booking.Application.Services;

public class LocalRoomService : ILocalRoomService
{
    private readonly ILocalRoomRepository _localRoomRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LocalRoomService(ILocalRoomRepository localRoomRepository, IUnitOfWork unitOfWork)
    {
        _localRoomRepository = localRoomRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> UpsertAsync(Guid roomId, string roomNumber, Guid roomTypeId, string status)
    {
        var existing = await _localRoomRepository.GetByIdAsync(roomId);

        if (existing is null)
        {
            var room = new LocalRoom(roomId, roomNumber, roomTypeId, status);
            await _localRoomRepository.AddAsync(room);
        }
        else
        {
            existing.Update(roomNumber, roomTypeId, status);
            _localRoomRepository.Update(existing);
        }

        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}
