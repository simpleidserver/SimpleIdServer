namespace SimpleIdServer.Authzen.Rego.Discover;

public interface IRegoPoliciesResolver
{
    string GetPolicyPath();
    Task<List<RegoPolicyRecord>?> Discover(CancellationToken cancellationToken = default(CancellationToken));
}

public class RegoPoliciesResolver : IRegoPoliciesResolver
{
    private readonly IRegoPathResolver _regoPathResolver;

    public RegoPoliciesResolver(IRegoPathResolver regoPathResolver)
    {
        _regoPathResolver = regoPathResolver;
    }

    public string GetPolicyPath() => _regoPathResolver.ResolvePoliciesPath();

    public async Task<List<RegoPolicyRecord>?> Discover(CancellationToken cancellationToken = default(CancellationToken))
    {
        var policyPath = _regoPathResolver.ResolvePoliciesPath();
        var policyConfiguration = PolicyConfigurationReader.ReadPolicy(policyPath);
        if (policyConfiguration == null)
        {
            return null;
        }

        var result = new List<RegoPolicyRecord>();
        var filePathLst = Directory.GetFiles(policyPath, $"*.{Constants.RegoFileExtension}", SearchOption.AllDirectories);
        var libs = new List<(string packageName, string path, List<(string packageName, string path)> otherLibs)>();
        foreach (var filePath in filePathLst)
        {
            var lines = await File.ReadAllLinesAsync(filePath, cancellationToken);
            var packageName = FilterInstructions(lines, Constants.RegoPackageInstr).FirstOrDefault(); ;
            if (packageName != null)
            {
                var libNames = FilterInstructions(lines, Constants.RegoImportInstr);
                var libRecs = libNames.SelectMany(libName =>
                {
                    var matchingLib = libs.FirstOrDefault(l => $"{Constants.RegoDataPackageNamePrefix}.{l.packageName}" == libName);
                    var r = new List<(string packageName, string filePath)>
                    {
                        (packageName: libName, filePath: matchingLib.path ?? string.Empty)
                    };
                    if (matchingLib.packageName == null)
                    {
                        return r;
                    }

                    r.AddRange(matchingLib.otherLibs);
                    return r;
                }).ToList();
                if (policyConfiguration.Entrypoints.Contains(packageName))
                {
                    var record = new RegoPolicyRecord
                    {
                        PolicyName = packageName,
                        Libs = libRecs,
                        Path = filePath
                    };
                    result.Add(record);
                }
                else
                {
                    libs.ForEach(r =>
                    {
                        var l = r.otherLibs.FirstOrDefault(l => l.packageName == packageName);
                        if (l.packageName != null)
                        {
                            l.path = filePath;
                        }
                    });
                    result.ForEach(r =>
                    {
                        var l = r.Libs.FirstOrDefault(l => l.packageName == packageName);
                        if (l.packageName != null)
                        {
                            l.filePath = filePath;
                        }
                    });
                    libs.Add((packageName, filePath, libRecs));
                }
            }
        }

        return result;
    }

    private static List<string> FilterInstructions(IEnumerable<string> lines, string instr)
    {
        return lines
            .Where(line => line.TrimStart().StartsWith(instr, StringComparison.InvariantCultureIgnoreCase))
            .Select(line => line.Substring(instr.Length).Trim())
            .ToList();
    }
}
