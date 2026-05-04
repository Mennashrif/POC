using System.Text.Json;
using RoomManagement.Application.Abstractions;
using RoomManagement.Domain.Abstractions;
using RoomManagement.Domain.Models;

namespace RoomManagement.Application.Services;

public class RoomTypeService : IRoomTypeService
{
    private readonly IRoomTypeRepository _roomTypeRepository;
    private readonly IOutboxRepository _outboxRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RoomTypeService(
        IRoomTypeRepository roomTypeRepository,
        IOutboxRepository outboxRepository,
        IUnitOfWork unitOfWork)
    {
        _roomTypeRepository = roomTypeRepository;
        _outboxRepository = outboxRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> AddAsync(string name, decimal price)
    {
        var roomType = new RoomType(name, price);

        await _roomTypeRepository.AddAsync(roomType);
        await _unitOfWork.SaveChangesAsync();

        return Result<Guid>.Success(roomType.Id);
    }

    public async Task<Result<bool>> UpdateAsync(Guid roomTypeId, string name, decimal price)
    {
        var roomType = await _roomTypeRepository.GetByIdAsync(roomTypeId);
        if (roomType is null)
            return Result<bool>.Failure($"RoomType {roomTypeId} not found.");

        roomType.Update(name, price);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

}
