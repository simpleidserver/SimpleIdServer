using FormBuilder.Rules;
using System.Text.Json.Serialization;

namespace FormBuilder.Models.Rules;


[JsonDerivedType(typeof(IncomingTokensTransformationRule), typeDiscriminator: "IncomingTokensTransRule")]
[JsonDerivedType(typeof(PropertyTransformationRule), typeDiscriminator: "PropertyTransformationRule")]
[JsonDerivedType(typeof(StaticValueTransformationRule), typeDiscriminator: "StaticValueTransformationRule")]
public interface ITransformationRule
{
    string Type { get; }
}
