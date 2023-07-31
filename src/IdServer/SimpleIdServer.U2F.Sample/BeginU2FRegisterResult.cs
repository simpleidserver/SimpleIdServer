using Fido2NetLib;
using System.Text.Json.Serialization;

namespace SimpleIdServer.U2F.Sample;

internal class BeginU2FRegisterResult
{
    [JsonPropertyName("session_id")]
    public string SessionId { get; set; } = null!;
    [JsonPropertyName("credential_create_options")]
    public CredentialCreateOptions CredentialCreateOptions { get; set; } = null!;
}
