namespace SimpleIdServer.Authzen.Rego;

public class RegoDownloaderOptions
{
    public required string GitHubUrl
    {
        get; set;
    } = "https://github.com/open-policy-agent/opa/releases/latest/download";

    public string? DownloadPath
    {
        get; set;
    } = null;
}
