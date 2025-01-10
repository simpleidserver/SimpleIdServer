using FormBuilder.Models;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.Input;

public class FormInputFieldRecord : BaseFormFieldRecord
{
    public override string Type => FormInputFieldDefinition.TYPE;
    public string Value { get; set; }
    public bool Disabled { get; set; }
    public FormInputTypes FormType { get; set; } = FormInputTypes.TEXT;

    public override void ExtractJson(JsonObject json)
        => json.Add(Name, Value);

    public override void Apply(JsonNode node)
        => Value = node.ToString();
}

public enum FormInputTypes
{
    TEXT = 0,
    HIDDEN = 1,
    PASSWORD = 2
}