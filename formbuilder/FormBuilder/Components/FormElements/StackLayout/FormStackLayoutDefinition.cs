
namespace FormBuilder.Components.FormElements.StackLayout;

public class FormStackLayoutDefinition : IFormElementDefinition
{
    public Type UiElt => typeof(FormStackLayout);

    public Type RecordType => typeof(FormStackLayoutRecord);
    public string Type => TYPE;
    public string Icon => "view_agenda";
    public static string TYPE = "Stacklayout";
    public ElementDefinitionCategories Category => ElementDefinitionCategories.LAYOUT;
}
