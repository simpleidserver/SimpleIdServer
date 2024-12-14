using System.Text.Json.Serialization;

namespace FormBuilder.Models.Rules;


[JsonDerivedType(typeof(IncomingTokensTransformationRule), typeDiscriminator: "IncomingTokensTransRule")]
public interface ITransformationRule
{
    string Type { get; }
}
