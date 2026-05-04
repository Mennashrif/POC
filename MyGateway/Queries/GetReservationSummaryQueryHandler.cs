using MediatR;
using MyGateway.DTOs;
using MyGateway.DTOs.Downstream;
using MyGateway.HttpClients;

namespace MyGateway.Queries;

public class GetReservationSummaryQueryHandler : IRequestHandler<GetReservationSummaryQuery, ReservationSummaryDto?>
{
    private readonly IBookingClient _bookingClient;
    private readonly IBillingClient _billingClient;

    public GetReservationSummaryQueryHandler(IBookingClient bookingClient, IBillingClient billingClient)
    {
        _bookingClient = bookingClient;
        _billingClient = billingClient;
    }

    public async Task<ReservationSummaryDto?> Handle(GetReservationSummaryQuery query, CancellationToken cancellationToken)
    {
        // fan-out
        var reservationTask = _bookingClient.GetReservationAsync(query.ReservationId);
        var billTask = _billingClient.GetBillByReservationAsync(query.ReservationId);

        await Task.WhenAll(reservationTask, billTask);

        var reservation = await reservationTask;
        if (reservation is null) return null;

        var bill = await billTask;

        // fan-in
        return new ReservationSummaryDto(
            reservation.Id,
            reservation.GuestName,
            reservation.Status,
            reservation.CheckIn,
            reservation.CheckOut,
            reservation.TotalRoomsRequested,
            bill is null ? null : new BillSummaryDto(bill.Id, bill.PhysicalRoomIds, bill.Status)
        );
    }
}
