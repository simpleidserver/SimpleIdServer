using FormBuilder.Models;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.Back;

public class BackButtonRecord : BaseFormFieldRecord
{
    public override string Type => BackButtonDefinition.TYPE;

    public override void Apply(JsonNode node)
    {
        
    }

    public override void ExtractJson(JsonObject json)
    {
    }
}
