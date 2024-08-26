using System.Text.Json.Serialization;

namespace SimpleIdServer.WalletClient.DTOs;

public class ErrorResult
{
    [JsonPropertyName("error")]
    public string Error { get; set; }
    [JsonPropertyName("error_description")]
    public string? ErrorDescription { get; set; } = null;
}
