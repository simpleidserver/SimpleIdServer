using Microsoft.Extensions.Options;
using SimpleIdServer.Authzen.Services;

namespace SimpleIdServer.Authzen.Rego;

public class RegoEvaluator : IAuthzenPolicyEvaluator
{
    private readonly RegoEvaluatorOptions _options;
    
    public RegoEvaluator(IOptions<RegoEvaluatorOptions> options)
    {
        _options = options.Value;
    }

    public Task<bool> Evaluate(Dictionary<string, object> input, CancellationToken cancellationToken)
    {
        var fullPath = Path.Combine(_options.PoliciesFolderBasePath, $"{_options.PolicyPath}.rego");
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Policy file not found: {fullPath}");
        }

        return Task.FromResult(true);
    }
}