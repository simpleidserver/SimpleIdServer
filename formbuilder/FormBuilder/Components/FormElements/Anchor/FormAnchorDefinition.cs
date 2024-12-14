
namespace FormBuilder.Components.FormElements.Anchor;

public class FormAnchorDefinition : IFormElementDefinition
{
    public Type UiElt => typeof(FormAnchor);
    public Type RecordType => typeof(FormAnchorRecord);
    public string Type => TYPE;
    public string Icon => "alternate_email";

    public static string TYPE = "FormAnchor";
    public ElementDefinitionCategories Category => ElementDefinitionCategories.ELEMENT;
}
