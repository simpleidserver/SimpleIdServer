using Fido2NetLib;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Mobile.DTOs;

public class BeginU2FRegisterResult
{
    [JsonPropertyName("session_id")]
    public string SessionId { get; set; } = null!;
    [JsonPropertyName("login")]
    public string Login { get; set; } = null!;
    [JsonPropertyName("credential_create_options")]
    public CredentialCreateOptions CredentialCreateOptions { get; set; } = null!;
    [JsonPropertyName("end_register_url")]
    public string EndRegisterUrl { get; set; } = null!;
}
