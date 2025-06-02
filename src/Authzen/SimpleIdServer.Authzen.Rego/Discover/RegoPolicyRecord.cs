namespace SimpleIdServer.Authzen.Rego.Discover;

public class RegoPolicyRecord
{
    public required string PolicyName
    {
        get; set;
    }

    public required string Path
    {
        get; set;
    }

    public required List<(string packageName, string filePath)> Libs
    {
        get; set;
    } = new List<(string packageName, string filePath)>();

    public string AllowPolicyName
    {
        get
        {
            return $"{PolicyName.Replace('.', '/')}/{Constants.RegoPolicyName}";
        }
    }

    public string AllPath
    {
        get
        {
            var result = Libs.Select(l => l.filePath).ToList();
            result.Add(Path);
            return string.Join(" ", result);
        }
    }
}
