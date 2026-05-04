using Billing.Application.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Text;
using ZXing;
using ZXing.ImageSharp;

namespace Billing.Infrastructure.Storage;

public class DataExtractor : IDataExtractor
{
    public async Task<string?> ExtractAsync(Stream fileStream, string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        if (extension == ".txt")
        {
            using var reader = new StreamReader(fileStream, Encoding.UTF8, leaveOpen: true);
            return await reader.ReadToEndAsync();
        }

        if (extension is ".png" or ".jpg" or ".jpeg")
        {
            return DecodeBarcode(fileStream);
        }

        return null;
    }

    private static string? DecodeBarcode(Stream stream)
    {
        using var image = Image.Load<Rgba32>(stream);
        var reader = new ZXing.ImageSharp.BarcodeReader<Rgba32>();
        var result = reader.Decode(image);
        return result?.Text;
    }
}
