using Billing.Application.Services;
using dnYara;
using Microsoft.Extensions.Configuration;

namespace Billing.Infrastructure.Storage;

public class YaraScanner : IYaraScanner, IDisposable
{
    private readonly CompiledRules _compiledRules;

    public YaraScanner(IConfiguration configuration)
    {
        var rulesPath = configuration["FileStorage:YaraRulesPath"]
            ?? Path.Combine(Directory.GetCurrentDirectory(), "Rules/malicious.yar");

        using var ctx = new YaraContext();
        using var compiler = new Compiler();
        compiler.AddRuleFile(rulesPath);
        _compiledRules = compiler.Compile();
    }

    public async Task<(bool IsMalicious, string? MatchedRule)> ScanAsync(Stream fileStream)
    {
        using var ms = new MemoryStream();
        await fileStream.CopyToAsync(ms);
        var buffer = ms.ToArray();

        var scanner = new Scanner();
        var results = scanner.ScanMemory(ref buffer, _compiledRules);

        if (results is not null && results.Count > 0)
            return (true, results[0].MatchingRule.Identifier);

        return (false, null);
    }

    public void Dispose()
    {
        _compiledRules.Dispose();
    }
}
