using RoomManagement.Domain.Abstractions;

namespace RoomManagement.Application.Services;

public interface IRoomTypeService
{
    Task<Result<Guid>> AddAsync(string name, decimal price);
    Task<Result<bool>> UpdateAsync(Guid roomTypeId, string name, decimal price);
}
