
namespace FormBuilder.Components.FormElements.Button;

public class FormButtonDefinition : IFormElementDefinition
{
    public Type UiElt => typeof(FormButton);
    public Type RecordType => typeof(FormButtonRecord);
}
