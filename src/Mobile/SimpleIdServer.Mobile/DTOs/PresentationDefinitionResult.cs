using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Mobile.DTOs
{
    public class PresentationDefinitionResult
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("input_descriptors")]
        public List<PresentationDefinitionInputDescriptorResult> InputDescriptors { get; set; }
    }

    public class PresentationDefinitionInputDescriptorResult
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("constraints")]
        public PresentationDefinitionConstraintResult Constraints { get; set; }
    }

    public class PresentationDefinitionConstraintResult
    {
        [JsonPropertyName("fields")]
        public List<PresentationDefinitionConstraintFieldResult> Fields { get; set; }
    }

    public class PresentationDefinitionConstraintFieldResult
    {
        [JsonPropertyName("path")]
        public List<string> Path { get; set; }
        [JsonPropertyName("filter")]
        public string Filter { get; set; }
    }
}
