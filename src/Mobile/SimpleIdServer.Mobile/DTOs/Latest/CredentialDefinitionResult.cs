using Java.Sql;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Mobile.DTOs.Latest;

public class CredentialDefinitionResult : BaseCredentialDefinitionResult
{
    [JsonPropertyName("type")]
    public IEnumerable<string> Type { get; set; }

    public override List<string> GetTypes()
        => Type?.ToList();
}