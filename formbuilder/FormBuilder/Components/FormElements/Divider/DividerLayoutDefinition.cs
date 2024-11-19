
namespace FormBuilder.Components.FormElements.Divider;

public class DividerLayoutDefinition : IFormElementDefinition
{
    public Type UiElt => typeof(DividerLayout);
    public Type RecordType => typeof(DividerLayoutRecord);
    public string Type => TYPE;
    public static string TYPE = "Divider";
    public ElementDefinitionCategories Category => ElementDefinitionCategories.LAYOUT;
}
