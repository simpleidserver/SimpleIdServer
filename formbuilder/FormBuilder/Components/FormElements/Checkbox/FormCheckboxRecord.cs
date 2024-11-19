using FormBuilder.Models;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.Checkbox;

public class FormCheckboxRecord : BaseFormFieldRecord
{
    public bool Value { get; set; }

    public override void ExtractJson(JsonObject json)
        => json.Add(Name, Value);

    public override void Apply(JsonNode node)
    {
        if (bool.TryParse(node?.ToString(), out bool value)) Value = value;
    }
}
