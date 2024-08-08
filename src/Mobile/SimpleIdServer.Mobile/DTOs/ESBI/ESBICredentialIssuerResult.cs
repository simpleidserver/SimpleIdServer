using SimpleIdServer.Mobile.DTOs.ESBI;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Mobile.DTOs;

public class ESBICredentialIssuer : BaseCredentialIssuer
{
    [JsonPropertyName("credentials_supported")]
    public List<ESBICredentialDefinitionResult> CredentialsSupported { get; set; }
}