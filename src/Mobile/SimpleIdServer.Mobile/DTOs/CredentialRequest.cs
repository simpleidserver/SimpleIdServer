using System.Text.Json.Serialization;

namespace SimpleIdServer.Mobile.DTOs;

public class CredentialRequest
{
    [JsonPropertyName("format")]
    public string Format { get; set; }
    [JsonPropertyName("credential_definition")]
    public CredentialDefinitionRequest CredentialDefinitionRequest { get; set; }
}

public class CredentialDefinitionRequest
{
    [JsonPropertyName("type")]
    public IEnumerable<string> Type { get; set; }
}