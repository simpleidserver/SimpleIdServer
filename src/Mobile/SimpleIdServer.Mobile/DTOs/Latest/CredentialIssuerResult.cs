using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Mobile.DTOs.Latest;

public class CredentialIssuerResult : BaseCredentialIssuer
{
    [JsonPropertyName("credential_configurations_supported")]
    public Dictionary<string, JsonObject> CredentialsConfigurationsSupported { get; set; } = new Dictionary<string, JsonObject>();
}
