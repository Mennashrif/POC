namespace Billing.Application.DTOs;

public record FileDownloadDto(
    Stream FileStream,
    string FileName,
    string ContentType,
    string Hash
);
