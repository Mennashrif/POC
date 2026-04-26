namespace Billing.Application.Abstractions;

public interface IDataExtractor
{
    Task<string?> ExtractAsync(Stream fileStream, string fileName);
}
