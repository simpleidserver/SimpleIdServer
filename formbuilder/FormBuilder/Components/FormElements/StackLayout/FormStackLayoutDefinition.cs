
namespace FormBuilder.Components.FormElements.StackLayout;

public class FormStackLayoutDefinition : IFormElementDefinition
{
    public Type UiElt => typeof(FormStackLayout);

    public Type RecordType => typeof(FormStackLayoutRecord);
}
