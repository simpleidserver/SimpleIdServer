using FormBuilder.Factories;

namespace FormBuilder.Components.FormElements.Checkbox;

public class FormCheckboxDefinition : BaseFormFieldElementDefinition<FormCheckboxRecord>
{
    public FormCheckboxDefinition(ITransformationRuleEngineFactory transformationRuleEngineFactory) : base(transformationRuleEngineFactory)
    {

    }

    public static string TYPE = "Checkbox";
    public override Type UiElt => typeof(FormCheckbox);
    public override string Type => TYPE;
    public override string Icon => "priority";
    public override ElementDefinitionCategories Category => ElementDefinitionCategories.ELEMENT;
}
