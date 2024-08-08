using System.Text.Json.Serialization;

namespace SimpleIdServer.Mobile.DTOs.ESBI;

public class ESBIGetCredentialRequest : BaseCredentialRequest
{
    [JsonPropertyName("types")]
    public List<string> Types { get; set; }
}