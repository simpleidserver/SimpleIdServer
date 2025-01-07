using FormBuilder.Models;
using FormBuilder.Models.Rules;
using System.Text.Json.Nodes;

namespace FormBuilder.Rules;

public abstract class GenericTransformationRule<T> : ITransformationRuleEngine where T : ITransformationRule
{
    public abstract string Type { get; }

    public void Apply<R>(R record, JsonObject input, ITransformationRule parameter) where R : IFormElementRecord
        => ProtectedApply(record, input, (T)parameter);

    protected abstract void ProtectedApply<R>(R record, JsonObject input, T parameter) where R : IFormElementRecord;
}
