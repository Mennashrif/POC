using MyGateway.DTOs.Downstream;

namespace MyGateway.HttpClients;

public interface IBookingClient
{
    Task<ReservationDto?> GetReservationAsync(Guid id);
}
