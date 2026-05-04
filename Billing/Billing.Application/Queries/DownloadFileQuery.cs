using Billing.Application.DTOs;
using Billing.Domain.Abstractions;
using MediatR;

namespace Billing.Application.Queries;

public record DownloadFileQuery(Guid FileId) : IRequest<Result<FileDownloadDto>>;
