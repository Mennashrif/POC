using System;
using Booking.Application.DTOs;
using MediatR;

namespace Booking.Application.Queries;

public record GetReservationByIdQuery(Guid Id) : IRequest<ReservationDto>;
