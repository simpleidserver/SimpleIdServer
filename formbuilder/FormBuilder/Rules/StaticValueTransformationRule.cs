using FormBuilder.Models.Rules;

namespace FormBuilder.Rules;

public class StaticValueTransformationRule : ITransformationRule
{
    public string Value { get; set; }
    public string Type => TYPE;
    public static string TYPE = "StaticValueTransRule";
}