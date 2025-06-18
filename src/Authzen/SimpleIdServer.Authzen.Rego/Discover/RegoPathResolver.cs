using Microsoft.Extensions.Options;

namespace SimpleIdServer.Authzen.Rego.Discover;

public interface IRegoPathResolver
{
    string ResolvePoliciesPath();
}

public class RegoPathResolver : IRegoPathResolver
{
    private readonly RegoOptions _options;

    public RegoPathResolver(IOptions<RegoOptions> options)
    {
        _options = options.Value;
    }

    public string ResolvePoliciesPath()
    {
        if (Path.IsPathRooted(_options.PoliciesFolderBasePath))
        {
            return _options.PoliciesFolderBasePath;
        }

        var assemblyLocation = Path.GetDirectoryName(typeof(RegoOptions).Assembly.Location);
        return Path.Combine(assemblyLocation!, _options.PoliciesFolderBasePath);
    }
}
