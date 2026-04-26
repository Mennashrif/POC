using Billing.Application.Services;
using Billing.Domain.Abstractions;
using MediatR;

namespace Billing.Application.Commands;

public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, Result<Guid>>
{
    private readonly IFileService _fileService;

    public UploadFileCommandHandler(IFileService fileService)
    {
        _fileService = fileService;
    }

    public Task<Result<Guid>> Handle(UploadFileCommand command, CancellationToken cancellationToken)
    {
        return _fileService.UploadAsync(command.FileStream, command.FileName, command.FileSize, command.ContentType);
    }
}
