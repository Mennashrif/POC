using Billing.Domain.Abstractions;
using MediatR;

namespace Billing.Application.Commands;

public record CancelBillCommand(Guid ReservationId) : IRequest<Result<bool>>;
