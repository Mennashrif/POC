using System;
using System.Collections.Generic;
using Billing.Domain.Abstractions;
using MediatR;

namespace Billing.Application.Commands;

public record CreateBillCommand(
    Guid ReservationId,
    string GuestName,
    List<string> PhysicalRoomIds,
    DateTime CheckInDate
) : IRequest<Result<Guid>>;
