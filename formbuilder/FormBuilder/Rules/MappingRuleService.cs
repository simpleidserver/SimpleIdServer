using FormBuilder.Models.Rules;
using Json.Path;
using System.Text.Json.Nodes;

namespace FormBuilder.Rules;

public interface IMappingRuleService
{
    JsonObject Extract(JsonObject data, IEnumerable<MappingRule> rules);
}

public class MappingRuleService : IMappingRuleService
{
    public JsonObject Extract(JsonObject data, IEnumerable<MappingRule> rules)
    {
        var result = new JsonObject();
        foreach (var rule in rules)
        {
            var path = JsonPath.Parse(rule.Source);
            var pathResult = path.Evaluate(data);
            var nodes = pathResult.Matches.Select(m => m.Value).Where(m => m != null);
            if (nodes.Count() != 1) continue;
            result.Add(rule.Target, nodes.Single().ToString());
        }

        return result;
    }
}
