namespace FormBuilder.Components.FormElements.Checkbox;

public class FormCheckboxDefinition : IFormElementDefinition
{
    public Type UiElt => typeof(FormCheckbox);

    public Type RecordType => typeof(FormCheckboxRecord);
    public string Type => TYPE;
    public string Icon => "priority";
    public static string TYPE = "Checkbox";
    public ElementDefinitionCategories Category => ElementDefinitionCategories.ELEMENT;
}
