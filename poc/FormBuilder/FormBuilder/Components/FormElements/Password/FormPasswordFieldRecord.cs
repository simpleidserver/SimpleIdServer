using FormBuilder.Models;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.Password;

public class FormPasswordFieldRecord : BaseFormFieldRecord
{
    public string Value { get; set; }

    public override void ExtractJson(JsonObject json)
        => json.Add(Name, Value);
}