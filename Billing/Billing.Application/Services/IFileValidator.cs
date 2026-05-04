namespace Billing.Application.Services;

public interface IFileValidator
{
    Task<(bool IsValid, string? Error)> ValidateAsync(Stream fileStream, string fileName, long fileSize);
}
