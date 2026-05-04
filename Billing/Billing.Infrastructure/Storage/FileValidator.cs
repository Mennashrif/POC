using Billing.Application.Services;
using MimeDetective;

namespace Billing.Infrastructure.Storage;

public class FileValidator : IFileValidator
{
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

    private static readonly string[] AllowedExtensions = [".txt", ".png", ".jpg", ".jpeg"];

    private static readonly Lazy<ContentInspectorBuilder> InspectorBuilder = new(() =>
        new ContentInspectorBuilder
        {
            Definitions = MimeDetective.Definitions.DefaultDefinitions.All()
        });

    private readonly IYaraScanner _yaraScanner;

    public FileValidator(IYaraScanner yaraScanner)
    {
        _yaraScanner = yaraScanner;
    }

    public async Task<(bool IsValid, string? Error)> ValidateAsync(Stream fileStream, string fileName, long fileSize)
    {
        // 1. Validate size
        if (fileSize > MaxFileSizeBytes)
            return (false, "File exceeds maximum allowed size of 5 MB.");

        // 2. Validate extension
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            return (false, $"Extension '{extension}' is not allowed. Allowed: {string.Join(", ", AllowedExtensions)}");

        // 3. Validate mime type using Mime-Detective (images only)
        if (extension != ".txt")
        {
            var (isValidImage, imageError) = ValidateMimeType(fileStream, extension);
            if (!isValidImage) return (false, imageError);
            fileStream.Position = 0;
        }

        // 5. YARA scan for malicious content (both txt and images)
        var (isMalicious, matchedRule) = await _yaraScanner.ScanAsync(fileStream);
        if (isMalicious)
            return (false, $"File contains malicious content. Matched rule: {matchedRule}");

        return (true, null);
    }

    private static (bool, string?) ValidateMimeType(Stream stream, string extension)
    {
        var inspector = InspectorBuilder.Value.Build();
        var results = inspector.Inspect(stream);
        if (results.Length == 0)
            return (false, $"Could not determine file type for '{extension}'.");

        var expectedMime = extension switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            _ => null
        };

        var match = results.Any(r => r.Definition.File.MimeType == expectedMime);
        if (!match)
            return (false, $"File content does not match expected {extension} format.");

        return (true, null);
    }

}
