using FormBuilder.Models;
using FormBuilder.Models.Rules;
using System.Text.Json.Nodes;

namespace FormBuilder.Rules;

public interface IRepetitionRuleEngine
{
    string Type { get; }
    List<IFormElementRecord> Transform(string fieldType, JsonObject input, IRepetitionRule parameter, Dictionary<string, object> parameters);
}
