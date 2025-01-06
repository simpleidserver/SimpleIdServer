using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Models;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.Image;

public class ImageRecord : IFormElementRecord
{
    public string Type => ImageDefinition.TYPE;
    public string Id { get; set; }
    public List<LabelTranslation> Labels { get; set; }
    public string Url { get; set; } = "https://simpleidserver.com/img/logo-no-shield.svg";
    public Dictionary<string, object> HtmlAttributes { get; set; } = new Dictionary<string, object>();

    public void ExtractJson(JsonObject json) { }

    public IFormElementRecord GetChild(string id)
        => null;
}
