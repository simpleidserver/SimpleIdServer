using System.Text.Json.Serialization;

namespace SimpleIdServer.Authzen.Dtos;

public class Subject
{
    [JsonPropertyName("type")]
    public required string Type
    {
        get; set;
    }

    [JsonPropertyName("id")]
    public required string Id
    {
        get; set;
    }

    [JsonPropertyName("properties")]
    public Dictionary<string, object>? Properties
    {
        get; set;
    } = null;
}
