using MediatR;
using MyGateway.DTOs;

namespace MyGateway.Queries;

public record GetReservationSummaryQuery(Guid ReservationId) : IRequest<ReservationSummaryDto?>;
