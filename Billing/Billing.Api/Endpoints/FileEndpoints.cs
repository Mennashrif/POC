using Billing.Application.Commands;
using Billing.Application.Queries;
using MediatR;

namespace Billing.Api.Endpoints;

public static class FileEndpoints
{
    public static WebApplication MapFileEndpoints(this WebApplication app)
    {
        app.MapPost("/bills/files/upload", async (IFormFile file, IMediator mediator) =>
        {
            await using var stream = file.OpenReadStream();

            var command = new UploadFileCommand(
                stream,
                file.FileName,
                file.Length,
                file.ContentType);

            var result = await mediator.Send(command);

            return result.IsSuccess
                ? Results.Ok(new { fileId = result.Value })
                : Results.BadRequest(new { error = result.Error });
        })
        .DisableAntiforgery()
        .WithName("UploadFile");

        app.MapGet("/bills/files/download/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var query = new DownloadFileQuery(id);
            var result = await mediator.Send(query);

            if (!result.IsSuccess)
                return Results.NotFound(new { error = result.Error });

            return Results.Stream(
                result.Value!.FileStream,
                result.Value.ContentType,
                result.Value.FileName,
                enableRangeProcessing: true);
        })
        .WithName("DownloadFile");

        return app;
    }
}
