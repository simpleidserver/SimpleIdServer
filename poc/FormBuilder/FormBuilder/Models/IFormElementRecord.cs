using System.Text.Json.Nodes;

namespace FormBuilder.Models;

public interface IFormElementRecord
{
    List<LabelTranslation> Labels { get; set; }
    void ExtractJson(JsonObject json);
}