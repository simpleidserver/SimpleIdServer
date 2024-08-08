using System.Text.Json.Serialization;

namespace SimpleIdServer.Mobile.DTOs.ESBI;

public class ESBICredentialResult  : BaseCredentialResult
{
    [JsonPropertyName("acceptance_token")]
    public string AcceptanceToken { get; set; }
}