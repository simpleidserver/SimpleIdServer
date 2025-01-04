using System.Text.Json.Nodes;

namespace FormBuilder.Conditions;

public interface IConditionRuleEngine
{
    string Type { get; }
    bool Evaluate(JsonObject input, IConditionParameter parameter);
}