namespace SimpleIdServer.Authzen.Rego.Discover;

public interface ICompiledOpaFilesResolver
{
    string GetCompiledOpaFolderPath();
    List<DiscoveredCompiledOpaFileResult> Resolve();
}

public class CompiledOpaFilesResolver : ICompiledOpaFilesResolver
{
    private readonly IRegoPathResolver _regoPathResolver;

    public CompiledOpaFilesResolver(IRegoPathResolver regoPathResolver)
    {
        _regoPathResolver = regoPathResolver;
    }

    public string GetCompiledOpaFolderPath()
    {
        var regoPath = _regoPathResolver.ResolvePoliciesPath();
        return Path.Combine(regoPath, Constants.WasmFileExtension);
    }

    public List<DiscoveredCompiledOpaFileResult> Resolve()
    {
        var result = new List<DiscoveredCompiledOpaFileResult>();
        var path = GetCompiledOpaFolderPath();
        if (!Directory.Exists(path))
        {
            return result;
        }

        var policyConfiguration = PolicyConfigurationReader.ReadPolicy(path);
        if (policyConfiguration == null)
        {
            return result;
        }

        foreach (var p in policyConfiguration.Entrypoints)
        {
            var compiledFilePath = Path.Combine(path, $"{p}.{Constants.WasmFileExtension}");
            if (!File.Exists(compiledFilePath))
            {
                continue;
            }

            result.Add(new DiscoveredCompiledOpaFileResult
            {
                FilePath = compiledFilePath,
                PackageName = Path.GetFileNameWithoutExtension(compiledFilePath)
            });
        }

        return result;
    }
}
