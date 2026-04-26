using Billing.Application.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Billing.Infrastructure.Storage;

public class LocalFileStorage : IFileStorage
{
    private readonly string _basePath;

    public LocalFileStorage(IConfiguration configuration)
    {
        _basePath = configuration["FileStorage:BasePath"]
            ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");

        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> SaveAsync(Stream fileStream, string fileName)
    {
        var uniqueName = $"{Guid.NewGuid()}_{fileName}";
        var fullPath = Path.Combine(_basePath, uniqueName);

        using var fileOut = File.Create(fullPath);
        await fileStream.CopyToAsync(fileOut);

        return fullPath;
    }

    public Task<Stream> GetAsync(string storagePath)
    {
        Stream stream = File.OpenRead(storagePath);
        return Task.FromResult(stream);
    }

    public bool Exists(string storagePath) => File.Exists(storagePath);
}
