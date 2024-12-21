
using FormBuilder.Factories;

namespace FormBuilder.Components.FormElements.Password;

public class FormPasswordFieldDefinition : BaseFormFieldElementDefinition<FormPasswordFieldRecord>
{
    public static string TYPE = "Password";

    public FormPasswordFieldDefinition(ITransformationRuleEngineFactory transformationRuleEngineFactory) : base(transformationRuleEngineFactory)
    {
    }

    public override Type UiElt => typeof(FormPasswordField);
    public override string Type => TYPE;
    public override string Icon => "password";
    public override ElementDefinitionCategories Category => ElementDefinitionCategories.ELEMENT;
}