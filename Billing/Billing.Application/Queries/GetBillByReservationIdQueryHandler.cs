using System.Threading;
using System.Threading.Tasks;
using Billing.Application.DTOs;
using Billing.Application.Services;
using MediatR;

namespace Billing.Application.Queries;

public class GetBillByReservationIdQueryHandler : IRequestHandler<GetBillByReservationIdQuery, BillDto?>
{
    private readonly IBillingService _billingService;

    public GetBillByReservationIdQueryHandler(IBillingService billingService)
    {
        _billingService = billingService;
    }

    public async Task<BillDto?> Handle(GetBillByReservationIdQuery request, CancellationToken cancellationToken)
    {
        return await _billingService.GetByReservationIdAsync(request.ReservationId);
    }
}
