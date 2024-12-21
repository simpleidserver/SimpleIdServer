using FormBuilder.Components;
using FormBuilder.Factories;

namespace FormBuilder.Rules;

public interface IRuleEngine
{
    void Apply(WorkflowContext context);
}

public class RuleEngine : IRuleEngine
{
    private readonly IFormElementDefinitionFactory _formElementDefinitionFactory;

    public RuleEngine(IFormElementDefinitionFactory formElementDefinitionFactory)
    {
        _formElementDefinitionFactory = formElementDefinitionFactory;
    }

    public void Apply(WorkflowContext context)
    {
        var formRecord = context.GetCurrentFormRecord();
        var definitions = _formElementDefinitionFactory.GetAll();
        foreach(var elt in formRecord.Elements)
        {
            var definition = definitions.Single(d => d.Type == elt.Type);
            definition.Init(elt, context, definitions);
        }
    }
}
