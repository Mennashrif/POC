using Billing.Application.DTOs;
using Billing.Domain.Abstractions;

namespace Billing.Application.Services;

public interface IFileService
{
    Task<Result<Guid>> UploadAsync(Stream fileStream, string fileName, long fileSize, string contentType);
    Task<Result<FileDownloadDto>> DownloadAsync(Guid fileId);
}
