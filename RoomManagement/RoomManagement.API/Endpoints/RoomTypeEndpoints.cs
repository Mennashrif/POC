using MediatR;
using RoomManagement.Application.Commands;
using RoomManagement.Application.Queries;

namespace RoomManagement.API.Endpoints;

public static class RoomTypeEndpoints
{
    public static WebApplication MapRoomTypeEndpoints(this WebApplication app)
    {
        app.MapGet("/roomtypes", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetAllRoomTypesQuery());
            return Results.Ok(result);
        })
        .WithName("GetAllRoomTypes");

        app.MapPost("/roomtypes", async (AddRoomTypeRequest request, IMediator mediator) =>
        {
            var command = new AddRoomTypeCommand(request.Name, request.Price);
            var result = await mediator.Send(command);

            return result.IsSuccess
                ? Results.Created($"/roomtypes/{result.Value}", new { id = result.Value })
                : Results.BadRequest(new { error = result.Error });
        })
        .WithName("AddRoomType");

        app.MapPut("/roomtypes/{id:guid}", async (Guid id, EditRoomTypeRequest request, IMediator mediator) =>
        {
            var command = new EditRoomTypeCommand(id, request.Name, request.Price);
            var result = await mediator.Send(command);

            return result.IsSuccess
                ? Results.Ok()
                : Results.BadRequest(new { error = result.Error });
        })
        .WithName("EditRoomType");

        return app;
    }
}

record AddRoomTypeRequest(string Name, decimal Price);
record EditRoomTypeRequest(string Name, decimal Price);
