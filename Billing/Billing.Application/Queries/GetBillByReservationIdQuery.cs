using System;
using Billing.Application.DTOs;
using MediatR;

namespace Billing.Application.Queries;

public record GetBillByReservationIdQuery(Guid ReservationId) : IRequest<BillDto?>;
