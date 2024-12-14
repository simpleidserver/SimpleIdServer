using FormBuilder.Models.Transformer;
using Microsoft.AspNetCore.Components.Rendering;

namespace FormBuilder.Transformers;

public interface ITransformer
{
    string Type { get; }
    object Transform(string value, ITransformerParameters transformerParameters);
    void BuildComponent(ITransformerParameters parameters, RenderTreeBuilder builder);
    ITransformerParameters CreateEmptyInstance();
}
