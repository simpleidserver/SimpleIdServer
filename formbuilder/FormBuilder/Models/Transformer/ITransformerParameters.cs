using FormBuilder.Transformers;
using System.Text.Json.Serialization;

namespace FormBuilder.Models.Transformer;

[JsonDerivedType(typeof(DirectTargetUrlTransformerParameters), typeDiscriminator: "DirectTargetUrlTransformer")]
[JsonDerivedType(typeof(RelativeUrlTransformerParameters), typeDiscriminator: "RelativeUrlTransformer")]
[JsonDerivedType(typeof(RegexTransformerParameters), typeDiscriminator: "RegexTransformer")]
public interface ITransformerParameters
{
    string Type { get; }
}