using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.WalletClient.DTOs.Latest;

public class CredentialIssuerResult : BaseCredentialIssuer
{
    [JsonPropertyName("credential_configurations_supported")]
    public Dictionary<string, JsonObject> CredentialsConfigurationsSupported { get; set; } = new Dictionary<string, JsonObject>();

    [JsonPropertyName("authorization_servers")]
    public List<string> AuthorizationServers { get; set; } = null;

    public override List<string> GetAuthorizationServers()
        => AuthorizationServers;
}
