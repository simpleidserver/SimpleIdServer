using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SimpleIdServer.Authzen.Rego.Discover;

public static class PolicyConfigurationReader
{
    public static PolicyConfiguration? ReadPolicy(string directoryPath)
    {
        if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath))
        {
            return null;
        }

        var filePath = Path.Combine(directoryPath, Constants.PolicyFileName);
        if (!File.Exists(filePath))
        {
            return null;
        }

        var yamlContent = File.ReadAllText(filePath);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var policy = deserializer.Deserialize<PolicyConfiguration>(yamlContent);
        return policy;
    }
}
