using MediatR;
using RoomManagement.Application.Commands;
using RoomManagement.Application.Queries;
using RoomManagement.Domain.Models;

namespace RoomManagement.API.Endpoints;

public static class RoomEndpoints
{
    public static WebApplication MapRoomEndpoints(this WebApplication app)
    {
        app.MapGet("/rooms", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetAllRoomsQuery());
            return Results.Ok(result);
        })
        .WithName("GetAllRooms");

        app.MapPost("/rooms", async (AddRoomRequest request, IMediator mediator) =>
        {
            var command = new AddRoomCommand(request.RoomNumber, request.RoomTypeId);
            var result = await mediator.Send(command);

            return result.IsSuccess
                ? Results.Created($"/rooms/{result.Value}", new { id = result.Value })
                : Results.BadRequest(new { error = result.Error });
        })
        .WithName("AddRoom");

        app.MapPut("/rooms/{id:guid}", async (Guid id, EditRoomRequest request, IMediator mediator) =>
        {
            var command = new EditRoomCommand(id, request.RoomNumber, request.RoomTypeId, request.Status);
            var result = await mediator.Send(command);

            return result.IsSuccess
                ? Results.Ok()
                : Results.BadRequest(new { error = result.Error });
        })
        .WithName("EditRoom");

        return app;
    }
}

record AddRoomRequest(string RoomNumber, Guid RoomTypeId);
record EditRoomRequest(string RoomNumber, Guid RoomTypeId, RoomStatus Status);
