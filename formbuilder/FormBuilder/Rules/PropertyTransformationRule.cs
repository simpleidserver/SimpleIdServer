using FormBuilder.Conditions;
using FormBuilder.Models.Rules;

namespace FormBuilder.Rules;

public class PropertyTransformationRule : ITransformationRule
{
    public string PropertyName { get; set; }
    public string PropertyValue { get; set; }
    public IConditionParameter Condition { get; set; }
    public string Type => TYPE;
    public static string TYPE = "PropertyTransRule";
}
