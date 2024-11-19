namespace FormBuilder.Components.FormElements.Input;

public class FormInputFieldDefinition : IFormElementDefinition
{
    public Type UiElt => typeof(FormInputField);
    public Type RecordType => typeof(FormInputFieldRecord);
    public string Type => TYPE;
    public static string TYPE = "Input";
    public ElementDefinitionCategories Category => ElementDefinitionCategories.ELEMENT;
}