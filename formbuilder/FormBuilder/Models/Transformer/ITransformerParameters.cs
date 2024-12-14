using FormBuilder.Transformers;
using System.Text.Json.Serialization;

namespace FormBuilder.Models.Transformer;

[JsonDerivedType(typeof(DirectTargetUrlTransformerParameters), typeDiscriminator: "DirectTargetUrlTransformer")]
public interface ITransformerParameters
{
    string Type { get; }
}