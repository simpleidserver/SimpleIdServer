using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SimpleIdServer.WalletClient.DTOs.ESBI;

public class ESBICredentialDefinitionResult : BaseCredentialDefinitionResult
{
    [JsonPropertyName("types")]
    public IEnumerable<string> Types { get; set; }

    public override List<string> GetTypes()
        => Types?.ToList();
}
