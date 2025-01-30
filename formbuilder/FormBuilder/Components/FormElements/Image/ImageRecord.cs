using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.Image;

public class ImageRecord : IFormElementRecord
{
    public string Type => ImageDefinition.TYPE;
    public string Id { get; set; }
    public string CorrelationId { get; set; }
    public List<LabelTranslation> Labels { get; set; }
    public string Url { get; set; } = "https://simpleidserver.com/img/logo-no-shield.svg";
    public Dictionary<string, object> HtmlAttributes { get; set; } = new Dictionary<string, object>();
    public List<ITransformationRule> Transformations { get; set; }
    public string CssStyle { get; set; }

    public void Apply(JsonNode node)
    {
        if (node == null) return;
        Url = node.ToString();
    }

    public void ExtractJson(JsonObject json) { }

    public IFormElementRecord GetChild(string id) => null;

    public IFormElementRecord GetChildByCorrelationId(string correlationId) => null;
}
