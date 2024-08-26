using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.WalletClient.DTOs.Latest;

public class GetCredentialRequest  : BaseCredentialRequest
{
    [JsonPropertyName("credential_definition")]
    public CredentialDefinitionRequest CredentialDefinitionRequest { get; set; }
}

public class CredentialDefinitionRequest
{
    [JsonPropertyName("type")]
    public IEnumerable<string> Type { get; set; }
}