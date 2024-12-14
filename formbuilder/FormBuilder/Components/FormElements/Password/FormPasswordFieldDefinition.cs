
namespace FormBuilder.Components.FormElements.Password;

public class FormPasswordFieldDefinition : IFormElementDefinition
{
    public Type UiElt => typeof(FormPasswordField);
    public Type RecordType => typeof(FormPasswordFieldRecord);
    public string Type => TYPE;
    public string Icon => "password";
    public static string TYPE = "Password";
    public ElementDefinitionCategories Category => ElementDefinitionCategories.ELEMENT;
}