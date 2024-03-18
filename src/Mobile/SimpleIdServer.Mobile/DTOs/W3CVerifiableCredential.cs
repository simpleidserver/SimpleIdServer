using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Mobile.DTOs;

public class W3CVerifiableCredential
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("@context")]
    public List<string> Context { get; set; } = new List<string>();
    [JsonPropertyName("type")]
    public List<string> Type { get; set; } = new List<string>();
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; } = null;
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; set; } = null;
    [JsonPropertyName("issuer")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Issuer { get; set; }
    [JsonPropertyName("validFrom")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? ValidFrom { get; set; }
    [JsonPropertyName("validUntil")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? ValidUntil { get; set; }
    [JsonPropertyName("credentialSubject")]
    public JsonNode CredentialSubject { get; set; }
}
