using System.Text.Json.Serialization;

namespace SimpleIdServer.Authzen.Dtos;

public class AccessEvaluationRequest
{
    [JsonPropertyName("subject")]
    public required Subject Subject
    {
        get; set;
    }

    [JsonPropertyName("action")]
    public required Action Action
    {
        get; set;
    }

    [JsonPropertyName("resource")]
    public required Resource Resource
    {
        get; set;
    }

    [JsonPropertyName("context")]
    public Dictionary<string, object>? Context
    {
        get; set;
    } = new Dictionary<string, object>();
}
