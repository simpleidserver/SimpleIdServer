
namespace FormBuilder.Components.FormElements.Button;

public class FormButtonDefinition : IFormElementDefinition
{
    public Type UiElt => typeof(FormButton);
    public Type RecordType => typeof(FormButtonRecord);
    public string Type => TYPE;
    public static string TYPE = "Button";
    public string Icon => "variables";
    public ElementDefinitionCategories Category => ElementDefinitionCategories.ELEMENT;
}
