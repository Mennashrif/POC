using MyGateway.DTOs.Downstream;

namespace MyGateway.HttpClients;

public interface IBillingClient
{
    Task<BillDto?> GetBillByReservationAsync(Guid reservationId);
}
