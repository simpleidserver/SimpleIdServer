using FormBuilder.Models.Rules;
using System.Text.Json.Nodes;

namespace FormBuilder.Models;

public abstract class BaseFormDataRecord : IFormElementRecord
{
    public abstract string Type { get; }
    public string FieldType { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
    public List<LabelTranslation> Labels { get; set; } = new List<LabelTranslation>();
    public Dictionary<string, object> HtmlAttributes { get; set; } = new Dictionary<string, object>();
    public List<ITransformationRule> Transformations { get; set; } = new List<ITransformationRule>();
    public string Id { get; set; }
    public string CorrelationId { get; set; }
    public string CssStyle { get; set; }

    public void Apply(JsonNode node)
    {

    }

    public void ExtractJson(JsonObject json)
    {

    }
    public IFormElementRecord GetChild(string id) => null;

    public IFormElementRecord GetChildByCorrelationId(string correlationId) => null;
}
