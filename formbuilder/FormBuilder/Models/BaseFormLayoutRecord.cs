using FormBuilder.Models.Rules;
using System.Collections.ObjectModel;
using System.Text.Json.Nodes;

namespace FormBuilder.Models;

public abstract class BaseFormLayoutRecord : IFormElementRecord
{
    public string Id { get; set; }
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

    public IFormElementRecord GetChild(string id) => Elements.SingleOrDefault(e => e.Id == id);
}
