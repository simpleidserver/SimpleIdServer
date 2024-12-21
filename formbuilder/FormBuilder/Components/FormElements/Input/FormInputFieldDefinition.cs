using FormBuilder.Factories;

namespace FormBuilder.Components.FormElements.Input;

public class FormInputFieldDefinition : BaseFormFieldElementDefinition<FormInputFieldRecord>
{
    public FormInputFieldDefinition(ITransformationRuleEngineFactory transformationRuleEngineFactory) : base(transformationRuleEngineFactory)
    {
        
    }

    public static string TYPE = "Input";
    public override Type UiElt => typeof(FormInputField);
    public override string Type => TYPE;
    public override string Icon => "text_fields";
    public override ElementDefinitionCategories Category => ElementDefinitionCategories.ELEMENT;
}