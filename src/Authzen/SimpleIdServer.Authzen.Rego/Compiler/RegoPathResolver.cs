using System;
using Microsoft.Extensions.Options;

namespace SimpleIdServer.Authzen.Rego.Compiler;

public interface IRegoPathResolver
{
    string ResolvePoliciesPath();
}

public class RegoPathResolver : IRegoPathResolver
{
    private readonly RegoEvaluatorOptions _options;

    public RegoPathResolver(IOptions<RegoEvaluatorOptions> options)
    {
        _options = options.Value;
    }

    public string ResolvePoliciesPath()
    {
        if (Path.IsPathRooted(_options.PoliciesFolderBasePath))
        {
            return _options.PoliciesFolderBasePath;
        }

        var assemblyLocation = Path.GetDirectoryName(typeof(RegoEvaluatorOptions).Assembly.Location);
        return Path.Combine(assemblyLocation!, _options.PoliciesFolderBasePath);
    }
}
