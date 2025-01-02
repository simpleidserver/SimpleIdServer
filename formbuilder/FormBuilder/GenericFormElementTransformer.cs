using FormBuilder.Models;
using System.Text.Json.Nodes;

namespace FormBuilder;

public abstract class GenericFormElementTransformer<T> : IFormElementTransformer where T : IFormElementRecord
{
    public abstract string Name { get; }

    public JsonNode Transform(JsonObject input, IFormElementRecord record)
        => ProtectedTransform(input, (T)record);

    protected abstract JsonNode ProtectedTransform(JsonObject input, T record);
}
