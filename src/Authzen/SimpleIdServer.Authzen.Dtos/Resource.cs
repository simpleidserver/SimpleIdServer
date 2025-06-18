using System.Text.Json.Serialization;

namespace SimpleIdServer.Authzen.Dtos;

public class Resource 
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
}