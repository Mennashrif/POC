using Billing.Domain.Abstractions;
using MediatR;

namespace Billing.Application.Commands;

public record UploadFileCommand(
    Stream FileStream,
    string FileName,
    long FileSize,
    string ContentType
) : IRequest<Result<Guid>>;
