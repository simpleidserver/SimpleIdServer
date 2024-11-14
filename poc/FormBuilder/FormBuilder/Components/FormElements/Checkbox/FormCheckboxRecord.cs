using FormBuilder.Models;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.Checkbox;

public class FormCheckboxRecord : BaseFormFieldRecord
{
    public bool Value { get; set; }

    public override void ExtractJson(JsonObject json)
        => json.Add(Name, Value);
}
