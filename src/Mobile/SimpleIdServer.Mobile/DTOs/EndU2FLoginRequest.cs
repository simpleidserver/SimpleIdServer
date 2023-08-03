using Fido2NetLib;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Mobile.DTOs
{
    public class EndU2FLoginRequest
    {
        [JsonPropertyName("login")]
        public string? Login { get; set; } = null;
        [JsonPropertyName("session_id")]
        public string SessionId { get; set; } = null!;
        [JsonPropertyName("assertion")]
        public AuthenticatorAssertionRawResponse Assertion { get; set; } = null!;
    }
}
