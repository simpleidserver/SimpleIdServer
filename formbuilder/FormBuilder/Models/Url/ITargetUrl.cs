using System.Text.Json.Serialization;

namespace FormBuilder.Models.Url;

[JsonDerivedType(typeof(DirectTargetUrl), typeDiscriminator: "DirectTarget")]
public interface ITargetUrl
{
    string Type { get; }
}