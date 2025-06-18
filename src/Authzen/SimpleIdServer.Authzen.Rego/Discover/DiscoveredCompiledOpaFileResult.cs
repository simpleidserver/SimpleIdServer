namespace SimpleIdServer.Authzen.Rego.Discover;

public class DiscoveredCompiledOpaFileResult
{
    public required string PackageName
    {
        get; set;
    }

    public required string FilePath
    {
        get; set;
    }
}
