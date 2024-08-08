using System.Text.Json.Serialization;

namespace SimpleIdServer.Mobile.DTOs;

public class ESBICredentialOffer : BaseCredentialOffer
{
    [JsonPropertyName("credentials")]
    public List<ESBICredential> Credentials { get; set; }
}

public class ESBICredential
{
    [JsonPropertyName("format")]
    public string Format { get; set; }
    [JsonPropertyName("types")]
    public List<string> Types { get; set; }
}