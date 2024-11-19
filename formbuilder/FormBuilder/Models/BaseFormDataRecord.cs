using System.Text.Json.Nodes;

namespace FormBuilder.Models;

public class BaseFormDataRecord : IFormElementRecord
{
    public string FieldType { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
    public List<LabelTranslation> Labels { get; set; } = new List<LabelTranslation>();

    public void ExtractJson(JsonObject json)
    {

    }
}
