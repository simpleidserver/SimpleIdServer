
using FormBuilder.Factories;

namespace FormBuilder.Components.FormElements.Anchor;

public class FormAnchorDefinition : BaseFormFieldElementDefinition<FormAnchorRecord>
{
    public FormAnchorDefinition(ITransformationRuleEngineFactory transformationRuleEngineFactory) : base(transformationRuleEngineFactory)
    {

    }


    public static string TYPE = "FormAnchor";
    public override Type UiElt => typeof(FormAnchor);
    public override string Type => TYPE;
    public override string Icon => "alternate_email";
    public override ElementDefinitionCategories Category => ElementDefinitionCategories.ELEMENT;
}
