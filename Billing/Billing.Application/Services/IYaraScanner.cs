namespace Billing.Application.Services;

public interface IYaraScanner
{
    Task<(bool IsMalicious, string? MatchedRule)> ScanAsync(Stream fileStream);
}
