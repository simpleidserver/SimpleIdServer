namespace FormBuilder.Components.FormElements.Input;

public class FormInputFieldDefinition : IFormElementDefinition
{
    public Type UiElt => typeof(FormInputField);
    public Type RecordType => typeof(FormInputFieldRecord);
}