using System.Threading;
using System.Threading.Tasks;
using Billing.Application.DTOs;
using Billing.Application.Services;
using MediatR;

namespace Billing.Application.Queries;

public class GetBillByIdQueryHandler : IRequestHandler<GetBillByIdQuery, BillDto?>
{
    private readonly IBillingService _billingService;

    public GetBillByIdQueryHandler(IBillingService billingService)
    {
        _billingService = billingService;
    }

    public async Task<BillDto?> Handle(GetBillByIdQuery request, CancellationToken cancellationToken)
    {
        return await _billingService.GetByIdAsync(request.Id);
    }
}
