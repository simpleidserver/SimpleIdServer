using System.Text.Json.Serialization;

namespace SimpleIdServer.Mobile.DTOs
{
    public class PresentationSubmissionRequest
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("definition_id")]
        public string DefinitionId { get; set; }
        [JsonPropertyName("descriptor_map")]
        public List<PresentationSubmissionDescriptorMapRequest> DescriptorMap { get; set; }
    }

    public class PresentationSubmissionDescriptorMapRequest
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("format")]
        public string Format { get; set; }
        [JsonPropertyName("path")]
        public string Path { get; set; } = "$";
        [JsonPropertyName("path_nested")]
        public PresentationSubmissionDescriptorMapPathNestedRequest PathNested { get; set; }
    }

    public class PresentationSubmissionDescriptorMapPathNestedRequest
    {
        [JsonPropertyName("format")]
        public string Format { get; set; }
        [JsonPropertyName("path")]
        public string Path { get; set; }
    }
}
