using MediatR;
using MyGateway.Queries;

public static class ReservationSummaryEndpoint
{
    public static WebApplication MapReservationSummaryEndpoint(this WebApplication app)
    {
        app.MapGet("/api/summary/reservation/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetReservationSummaryQuery(id));

            return result is not null
                ? Results.Ok(result)
                : Results.NotFound(new { error = $"Reservation {id} not found." });
        })
        .RequireRateLimiting("fixed")
        .RequireAuthorization()
        .WithName("GetReservationSummary");

        return app;
    }
}
