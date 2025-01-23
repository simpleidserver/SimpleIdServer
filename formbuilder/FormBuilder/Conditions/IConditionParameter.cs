using System.Text.Json.Serialization;

namespace FormBuilder.Conditions;

[JsonDerivedType(typeof(PresentParameter), typeDiscriminator: "PresentParameter")]
[JsonDerivedType(typeof(ComparisonParameter), typeDiscriminator: "ComparisonParameter")]
[JsonDerivedType(typeof(LogicalParameter), typeDiscriminator: "LogicalParameter")]
[JsonDerivedType(typeof(UserAuthenticatedParameter), typeDiscriminator: "UserAuthenticatedParameter")]
[JsonDerivedType(typeof(NotPresentParameter), typeDiscriminator: "NotPresentParameter")]
public interface IConditionParameter
{
    public string Type { get; }
}
