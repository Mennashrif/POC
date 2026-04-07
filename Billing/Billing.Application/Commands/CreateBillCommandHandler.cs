using System.Threading;
using System.Threading.Tasks;
using Billing.Application.Services;
using Billing.Domain.Abstractions;
using MediatR;

namespace Billing.Application.Commands;

public class CreateBillCommandHandler : IRequestHandler<CreateBillCommand, Result<Guid>>
{
    private readonly IBillingService _billingService;

    public CreateBillCommandHandler(IBillingService billingService)
    {
        _billingService = billingService;
    }

    public async Task<Result<Guid>> Handle(CreateBillCommand request, CancellationToken cancellationToken)
    {
        return await _billingService.CreateBillAsync(
            request.ReservationId,
            request.GuestName,
            request.PhysicalRoomIds,
            request.CheckInDate);
    }
}
