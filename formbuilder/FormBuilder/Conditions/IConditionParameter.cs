using System.Text.Json.Serialization;

namespace FormBuilder.Conditions;

[JsonDerivedType(typeof(PresentParameter), typeDiscriminator: "PresentParameter")]
public interface IConditionParameter
{
    public string Type { get; }
}
