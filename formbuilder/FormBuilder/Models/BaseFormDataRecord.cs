using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace FormBuilder.Models;

public abstract class BaseFormDataRecord : IFormElementRecord
{
    public abstract string Type { get; }
    public string FieldType { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
    public List<LabelTranslation> Labels { get; set; } = new List<LabelTranslation>();
    public Dictionary<string, object> HtmlAttributes { get; set; } = new Dictionary<string, object>();
    public string Id { get; set; }

    public void ExtractJson(JsonObject json)
    {

    }
    public IFormElementRecord GetChild(string id) => null;
}
