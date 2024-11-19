namespace FormBuilder.Components.FormElements.Checkbox;

public class FormCheckboxDefinition : IFormElementDefinition
{
    public Type UiElt => typeof(FormCheckbox);

    public Type RecordType => typeof(FormCheckboxRecord);
    public string Type => TYPE;
    public static string TYPE = "Checkbox";
}
