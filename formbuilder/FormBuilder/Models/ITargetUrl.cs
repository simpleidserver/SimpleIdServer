using System.Text.Json.Serialization;

namespace FormBuilder.Models;

[JsonDerivedType(typeof(DirectTargetUrl), typeDiscriminator: "DirectTarget")]
[JsonDerivedType(typeof(ControllerActionTargetUrl), typeDiscriminator: "ControllerAction")]
public interface ITargetUrl
{
    string Type { get; }
}