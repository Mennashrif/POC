using Billing.Application.Services;
using Billing.Domain.Abstractions;
using MediatR;

namespace Billing.Application.Commands;

public class CancelBillCommandHandler : IRequestHandler<CancelBillCommand, Result<bool>>
{
    private readonly IBillingService _billingService;

    public CancelBillCommandHandler(IBillingService billingService)
    {
        _billingService = billingService;
    }

    public async Task<Result<bool>> Handle(CancelBillCommand request, CancellationToken cancellationToken)
        => await _billingService.CancelBillAsync(request.ReservationId);
}
