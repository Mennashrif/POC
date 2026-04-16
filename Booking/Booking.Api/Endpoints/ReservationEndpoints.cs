using Booking.Application.Commands;
using Booking.Application.Queries;
using Booking.Domain.Models;
using MediatR;
using System;

namespace Booking.Api.Endpoints;

public static class ReservationEndpoints
{
    public static WebApplication MapReservationEndpoints(this WebApplication app)
    {
        app.MapPost("/reservations", async (CreateReservationRequest request, IMediator mediator) =>
        {
            var command = new CreateReservationCommand(
                new GuestDetails(request.Guest.Name, request.Guest.Email, request.Guest.Phone),
                request.CheckIn,
                request.CheckOut,
                request.RoomRequests.Select(r => new RoomRequest(r.RoomTypeId, r.Quantity)).ToList()
            );

            var result = await mediator.Send(command);

            return result.IsSuccess
                ? Results.Created($"/reservations/{result.Value}", new { id = result.Value })
                : Results.Conflict(new { error = result.Error });
        })
        .WithName("CreateReservation");

        app.MapGet("/reservations/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var query = new GetReservationByIdQuery(id);
            var dto = await mediator.Send(query);

            return dto is not null
                ? Results.Ok(dto)
                : Results.NotFound(new { error = $"Reservation {id} not found." });
        })
        .WithName("GetReservationById");

        app.MapPut("/reservations/{id:guid}/checkin", async (Guid id, CheckInRequest request, IMediator mediator) =>
        {
            var command = new CheckInReservationCommand(id, request.PhysicalRoomIds);
            var result = await mediator.Send(command);

            return result.IsSuccess
                ? Results.Ok()
                : Results.BadRequest(new { error = result.Error });
        })
        .WithName("CheckInReservation");

        return app;
    }
}

// ─── Request Models (API boundary — never domain objects) ─────────────────────

record CreateReservationRequest(
    GuestRequest Guest,
    DateTime CheckIn,
    DateTime CheckOut,
    List<RoomRequestDto> RoomRequests
);

record GuestRequest(string Name, string Email, string Phone);

record RoomRequestDto(Guid RoomTypeId, int Quantity = 1);

record CheckInRequest(List<string> PhysicalRoomIds);

