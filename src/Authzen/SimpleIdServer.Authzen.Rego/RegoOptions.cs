namespace SimpleIdServer.Authzen.Rego;

public class RegoOptions
{
    public required string PoliciesFolderBasePath
    {
        get; set;
    } = "policies";

    public required string PolicyPath
    {
        get; set;
    } = "authzen.evaluate.allow";
    
    public required string GitHubUrl
    {
        get; set;
    } = "https://github.com/open-policy-agent/opa/releases/latest/download";

    public string? DownloadPath
    {
        get; set;
    } = null;
}