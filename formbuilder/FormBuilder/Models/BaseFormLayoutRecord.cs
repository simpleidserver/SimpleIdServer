using FormBuilder.Models.Rules;
using System.Collections.ObjectModel;
using System.Text.Json.Nodes;

namespace FormBuilder.Models;

public abstract class BaseFormLayoutRecord : IFormElementRecord
{
    public string Id { get; set; }
    public string CorrelationId { get; set; }
    public string CssStyle { get; set; } = "";
    public abstract string Type { get; }
    public ObservableCollection<IFormElementRecord> Elements { get; set; }
    public Dictionary<string, object> HtmlAttributes { get; set; } = new Dictionary<string, object>();
    public List<LabelTranslation> Labels { get; set; } = new List<LabelTranslation>();
    public List<ITransformationRule> Transformations { get; set; }

    public void Apply(JsonNode node)
    {

    }

    public void ExtractJson(JsonObject json)
    {
        foreach(var elt in Elements)
            elt.ExtractJson(json);
    }

    public List<IFormElementRecord> GetChildrenBranch(string id)
    {
        var filtered = Elements.SingleOrDefault(e => e.Id == id);
        if (filtered != null) return new List<IFormElementRecord> { this, filtered };
        var result = new List<IFormElementRecord>();
        foreach(var elt in Elements)
        {
            var children = elt.GetChildrenBranch(id);
            if(children.Any())
            {
                result.Add(this);
                result.AddRange(children);
                return result;
            }
        }

        return result;
    }

    public IFormElementRecord GetChildByCorrelationId(string correlationId) => Elements.SingleOrDefault(e => e.CorrelationId == correlationId);
}
