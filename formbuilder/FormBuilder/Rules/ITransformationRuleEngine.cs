using FormBuilder.Models;
using FormBuilder.Models.Rules;
using System.Text.Json.Nodes;

namespace FormBuilder.Rules;

public interface ITransformationRuleEngine
{
    string Type { get; }
    void Apply<R>(R record, JsonObject input, ITransformationRule parameter) where R : IFormElementRecord;
}