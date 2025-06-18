using System.Text.Json.Serialization;

namespace SimpleIdServer.Authzen.Dtos;

public class Action
{
    [JsonPropertyName("name")]
    public required string Name
    {
        get; set;
    }

    [JsonPropertyName("properties")]
    public Dictionary<string, object>? Properties
    {
        get; set;
    } = new Dictionary<string, object>();
}