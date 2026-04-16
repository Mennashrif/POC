namespace RoomManagement.Application.Abstractions;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}
