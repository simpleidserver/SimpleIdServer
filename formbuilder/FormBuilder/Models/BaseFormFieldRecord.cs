using System.Text.Json.Nodes;

namespace FormBuilder.Models;

public abstract class BaseFormFieldRecord : IFormElementRecord
{
    public string Name { get; set; }
    public List<LabelTranslation> Labels { get; set; } = new List<LabelTranslation>();
    public abstract void ExtractJson(JsonObject json);
}