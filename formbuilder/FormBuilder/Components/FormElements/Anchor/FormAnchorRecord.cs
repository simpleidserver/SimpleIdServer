using FormBuilder.Models;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.Anchor;

public class FormAnchorRecord : BaseFormFieldRecord
{
    public override string Type => FormAnchorDefinition.TYPE;
    
    public bool ActAsButton 
    { 
        get; set; 
    } = false;

    public bool IsStaticLink
    {
        get; set;
    } = false;

    public string Url
    {
        get; set;
    }

    public bool OpenInNewTab 
    { 
        get; set; 
    } = false;

    public override void Apply(JsonNode node)
    {
    }

    public override void ExtractJson(JsonObject json)
    {
    }
}
