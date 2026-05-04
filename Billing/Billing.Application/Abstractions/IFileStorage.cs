namespace Billing.Application.Abstractions;

public interface IFileStorage
{
    Task<string> SaveAsync(Stream fileStream, string fileName);
    Task<Stream> GetAsync(string storagePath);
    bool Exists(string storagePath);
}
