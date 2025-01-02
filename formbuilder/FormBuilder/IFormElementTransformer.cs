using FormBuilder.Models;
using System.Text.Json.Nodes;

namespace FormBuilder;

public interface IFormElementTransformer
{
    string Name { get; }
    JsonNode Transform(JsonObject input, IFormElementRecord record);
}
