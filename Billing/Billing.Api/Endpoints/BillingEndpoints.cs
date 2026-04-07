using Billing.Application.Queries;
using MediatR;

namespace Billing.Api.Endpoints;

public static class BillingEndpoints
{
    public static WebApplication MapBillingEndpoints(this WebApplication app)
    {
        app.MapGet("/bills/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var query = new GetBillByIdQuery(id);
            var dto = await mediator.Send(query);

            return dto is not null
                ? Results.Ok(dto)
                : Results.NotFound(new { error = $"Bill {id} not found." });
        })
        .WithName("GetBillById");

        app.MapGet("/bills/reservation/{reservationId:guid}", async (Guid reservationId, IMediator mediator) =>
        {
            var query = new GetBillByReservationIdQuery(reservationId);
            var dto = await mediator.Send(query);

            return dto is not null
                ? Results.Ok(dto)
                : Results.NotFound(new { error = $"No bill found for reservation {reservationId}." });
        })
        .WithName("GetBillByReservationId");

        return app;
    }
}
