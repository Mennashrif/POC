using Billing.Domain.Abstractions;

namespace Billing.Domain.Models;

public class BillingFile : BaseEntity<Guid>
{
    public string FileName { get; private set; }
    public string ContentType { get; private set; }
    public long FileSize { get; private set; }
    public string Hash { get; private set; }
    public string StoragePath { get; private set; }
    public DateTime UploadedAt { get; private set; }
    public string? ExtractedData { get; private set; }

    private BillingFile() : base(Guid.Empty) { }

    public BillingFile(string fileName, string contentType, long fileSize, string hash, string storagePath)
        : base(Guid.NewGuid())
    {
        FileName = fileName;
        ContentType = contentType;
        FileSize = fileSize;
        Hash = hash;
        StoragePath = storagePath;
        UploadedAt = DateTime.UtcNow;
    }

    public void SetExtractedData(string data)
    {
        ExtractedData = data;
    }
}
