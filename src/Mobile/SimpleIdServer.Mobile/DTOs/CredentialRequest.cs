using System.Text.Json.Serialization;

namespace SimpleIdServer.Mobile.DTOs
{
    public class CredentialRequest
    {
        [JsonPropertyName("format")]
        public string Format { get; set; }
    }
}
