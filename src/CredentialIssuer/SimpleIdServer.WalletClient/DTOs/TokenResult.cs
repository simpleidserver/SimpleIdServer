using System.Text.Json.Serialization;

namespace SimpleIdServer.WalletClient.DTOs;

public class TokenResult
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
    [JsonPropertyName("id_token")]
    public string IdToken { get; set; }
    [JsonPropertyName("c_nonce")]
    public string CNonce { get; set; }
}
