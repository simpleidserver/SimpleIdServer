using FormBuilder.Transformers;
using System.Text.Json.Serialization;

namespace FormBuilder.Models.Transformer;

[JsonDerivedType(typeof(ControllerActionTransformerParameters), typeDiscriminator: "ControllerActionTransformer")]
public interface ITransformerParameters
{
    string Type { get; }
}