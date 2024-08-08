using System.Text.Json.Serialization;

namespace SimpleIdServer.Mobile.DTOs;

public class BaseCredentialIssuer
{
    [JsonPropertyName("credential_issuer")]
    public string CredentialIssuer { get; set; } = null!;

    [JsonPropertyName("authorization_servers")]
    public IEnumerable<string> AuthorizationServers { get; set; } = null;
    [JsonPropertyName("credential_endpoint")]
    public string CredentialEndpoint { get; set; } = null!;
    [JsonPropertyName("deferred_credential_endpoint")]
    public string DeferredCredentialEndpoint { get; set; }
}