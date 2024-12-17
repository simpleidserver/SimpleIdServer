using FormBuilder.Models.Transformer;
using Microsoft.AspNetCore.Components.Rendering;
using System.Text.Json.Nodes;

namespace FormBuilder.Transformers;

public interface ITransformer
{
    string Type { get; }
    object Transform(string value, ITransformerParameters transformerParameters, JsonNode data);
    void BuildComponent(ITransformerParameters parameters, RenderTreeBuilder builder);
    ITransformerParameters CreateEmptyInstance();
}
