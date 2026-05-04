using RoomManagement.Application.DTOs;
using RoomManagement.Domain.Models;

namespace RoomManagement.Application.Abstractions;

public interface IRoomTypeRepository
{
    Task<RoomType?> GetByIdAsync(Guid id);
    Task<List<RoomTypeDto>> GetAllAsync();
    Task AddAsync(RoomType roomType);
}
