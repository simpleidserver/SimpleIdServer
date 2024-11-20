using System.Collections.ObjectModel;
using System.Text.Json.Nodes;

namespace FormBuilder.Models;

public abstract class BaseFormLayoutRecord : IFormElementRecord
{
    public ObservableCollection<IFormElementRecord> Elements { get; set; }
    public List<LabelTranslation> Labels { get; set; } = new List<LabelTranslation>();

    public void ExtractJson(JsonObject json)
    {
        foreach(var elt in Elements)
            elt.ExtractJson(json);
    }
}
