using RoomManagement.Application.DTOs;
using RoomManagement.Domain.Models;

namespace RoomManagement.Application.Abstractions;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(Guid id);
    Task<Room?> GetByRoomNumberAsync(string roomNumber);
    Task<List<RoomDto>> GetAllAsync();
    Task AddAsync(Room room);
    void Update(Room room);
    void UpdateRange(List<Room> rooms);
    Task<List<Room>> GetByRoomNumbersAsync(List<string> physicalRoomNumbers);
}
