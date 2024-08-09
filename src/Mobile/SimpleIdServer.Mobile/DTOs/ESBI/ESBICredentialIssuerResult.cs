using SimpleIdServer.Mobile.DTOs.ESBI;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Mobile.DTOs;

public class ESBICredentialIssuer : BaseCredentialIssuer
{
    [JsonPropertyName("credentials_supported")]
    public List<ESBICredentialDefinitionResult> CredentialsSupported { get; set; }
    [JsonPropertyName("authorization_server")]
    public string AuthorizationServer { get; set; }

    public override List<string> GetAuthorizationServers()
        => new List<string> { AuthorizationServer };
}