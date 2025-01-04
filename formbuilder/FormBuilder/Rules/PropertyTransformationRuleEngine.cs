using FormBuilder.Factories;
using System.Text.Json.Nodes;

namespace FormBuilder.Rules;

public class PropertyTransformationRuleEngine : GenericTransformationRule<PropertyTransformationRule>
{
    private readonly IConditionRuleEngineFactory _conditionRuleEngineFactory;

    public PropertyTransformationRuleEngine(IConditionRuleEngineFactory conditionRuleEngineFactory)
    {
        _conditionRuleEngineFactory = conditionRuleEngineFactory;
    }

    public override string Type => PropertyTransformationRule.TYPE;

    protected override void ProtectedApply<R>(R record, JsonObject input, PropertyTransformationRule parameter)
    {
        if(parameter.Condition != null)
        {
            var engine = _conditionRuleEngineFactory.Build(parameter.Condition);
            if (!engine.Evaluate(input, parameter.Condition)) return;
        }

        var type = typeof(R);
        var property = type.GetProperty(parameter.PropertyName);
        property.SetValue(record, Convert.ChangeType(parameter.PropertyValue, property.PropertyType));
    }
}