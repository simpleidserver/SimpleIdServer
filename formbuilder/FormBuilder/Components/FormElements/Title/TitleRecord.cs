using FormBuilder.Models;
using Radzen.Blazor;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.Title;

public class TitleRecord : IFormElementRecord
{
    public string Type => TitleDefinition.TYPE;
    public string Id { get; set; }
    public string CorrelationId {  get; set; }
    public string CssStyle { get; set; }
    public List<LabelTranslation> Labels { get; set; } = new List<LabelTranslation>();
    public TextStyle Style { get; set; } = TextStyle.H5;
    public Dictionary<string, object> HtmlAttributes { get; set; } = new Dictionary<string, object>();

    public void Apply(JsonNode node)
    {
    }

    public void ExtractJson(JsonObject json)
    {
    }

    public IFormElementRecord GetChild(string id) => null;

    public IFormElementRecord GetChildByCorrelationId(string correlationId) => null;
}
