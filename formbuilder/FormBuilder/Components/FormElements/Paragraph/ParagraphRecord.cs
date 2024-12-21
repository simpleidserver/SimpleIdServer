using FormBuilder.Components.FormElements.ListData;
using FormBuilder.Models;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.Paragraph;

public class ParagraphRecord : IFormElementRecord
{
    public string Type => ParagraphDefinition.TYPE;
    public string Id { get; set; }
    public List<LabelTranslation> Labels { get; set; } = new List<LabelTranslation>();

    public void ExtractJson(JsonObject json)
    {
    }

    public IFormElementRecord GetChild(string id) => null;
}
