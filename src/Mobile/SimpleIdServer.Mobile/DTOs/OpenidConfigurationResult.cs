using System.Text.Json.Serialization;

namespace SimpleIdServer.Mobile.DTOs;

public class OpenidConfigurationResult
{
    [JsonPropertyName("issuer")]
    public string Issuer { get; set; }
    [JsonPropertyName("authorization_endpoint")]
    public string AuthorizationEndpoint { get; set; }
    [JsonPropertyName("token_endpoint")]
    public string TokenEndpoint { get; set; }
}
