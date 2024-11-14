using FormBuilder.Models;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.Input;

public class FormInputFieldRecord : BaseFormFieldRecord
{
    public string Value { get; set; }

    public override void ExtractJson(JsonObject json)
        => json.Add(Name, Value);
}