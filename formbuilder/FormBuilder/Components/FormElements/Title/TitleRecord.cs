using FormBuilder.Models;
using Radzen.Blazor;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.Title;

public class TitleRecord : IFormElementRecord
{
    public string Id { get; set; }
    public List<LabelTranslation> Labels { get; set; } = new List<LabelTranslation>();
    public TextStyle Style { get; set; } = TextStyle.H5;

    public void ExtractJson(JsonObject json)
    {
    }

    public IFormElementRecord GetChild(string id) => null;
}
