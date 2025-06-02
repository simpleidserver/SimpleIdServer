namespace SimpleIdServer.Authzen.Rego.Discover;

public class PolicyConfiguration
{
    public string? Name
    {
        get; set;
    }

    public string? Description
    {
        get; set;
    }

    public string? Root
    {
        get; set;
    }

    public List<string> Entrypoints
    {
        get; set;
    } = new List<string>();
}
