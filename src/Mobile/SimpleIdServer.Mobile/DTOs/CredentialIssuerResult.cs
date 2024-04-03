using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Mobile.DTOs;

public class CredentialIssuerResult
{
    [JsonPropertyName("credential_issuer")]
    public string CredentialIssuer { get; set; } = null!;

    [JsonPropertyName("authorization_servers")]
    public IEnumerable<string> AuthorizationServers { get; set; } = null;
    [JsonPropertyName("credential_endpoint")]
    public string CredentialEndpoint { get; set; } = null!;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("credential_response_encryption_alg_values_supported")]
    public IEnumerable<string> CredentialResponseEncryptionAlgValuesSupported { get; set; } = null;
    [JsonPropertyName("credential_response_encryption_enc_values_supported")]
    public IEnumerable<string> CredentialResponseEncryptionEncValuesSupported { get; set; } = null;
    [JsonPropertyName("require_credential_response_encryption")]
    public bool RequireCredentialResponseEncryption { get; set; }
    [JsonPropertyName("credential_identifiers_supported")]
    public bool CredentialIdentifiersSupported { get; set; }
    [JsonPropertyName("credentials_supported")]
    public Dictionary<string, JsonObject> CredentialsSupported { get; set; } = new Dictionary<string, JsonObject>();
}
