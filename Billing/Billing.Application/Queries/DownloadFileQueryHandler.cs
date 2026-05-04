using Billing.Application.DTOs;
using Billing.Application.Services;
using Billing.Domain.Abstractions;
using MediatR;

namespace Billing.Application.Queries;

public class DownloadFileQueryHandler : IRequestHandler<DownloadFileQuery, Result<FileDownloadDto>>
{
    private readonly IFileService _fileService;

    public DownloadFileQueryHandler(IFileService fileService)
    {
        _fileService = fileService;
    }

    public Task<Result<FileDownloadDto>> Handle(DownloadFileQuery query, CancellationToken cancellationToken)
    {
        return _fileService.DownloadAsync(query.FileId);
    }
}
