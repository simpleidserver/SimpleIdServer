using System.Text.Json.Nodes;

namespace SimpleIdServer.Mobile.DTOs
{
    public class PresentationDefinitionResult
    {
        public string Id { get; set; }
        public List<PresentationDefinitionInputDescriptorResult> InputDescriptors { get; set; }

        public PresentationDefinitionResult Extract(JsonObject jsonObj)
        {
            return null;
        }
    }

    public class PresentationDefinitionInputDescriptorResult
    {
        public string Id { get; set; }
        public string Type { get; set; }

        public static PresentationDefinitionInputDescriptorResult Extract(JsonObject jsonObj)
        {
            return null;
        }
    }
}
