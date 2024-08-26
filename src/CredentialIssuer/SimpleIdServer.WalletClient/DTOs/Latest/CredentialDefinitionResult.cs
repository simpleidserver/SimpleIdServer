using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SimpleIdServer.WalletClient.DTOs.Latest;

public class CredentialDefinitionResult : BaseCredentialDefinitionResult
{
    [JsonPropertyName("type")]
    public IEnumerable<string> Type { get; set; }

    public override List<string> GetTypes()
        => Type?.ToList();
}