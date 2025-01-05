using System.Text.Json.Nodes;

namespace FormBuilder.Rules;

public class StaticValueTransformationRuleEngine : GenericTransformationRule<StaticValueTransformationRule>
{
    public override string Type => StaticValueTransformationRule.TYPE;

    protected override void ProtectedApply<R>(R record, JsonObject input, StaticValueTransformationRule parameter)
    {
        record.Apply(parameter.Value);
    }
}
