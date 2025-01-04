namespace FormBuilder.Conditions;

public class ComparisonParameter : IConditionParameter
{
    public static string TYPE = "Comparison";
    public string Source { get; set; }
    public string Value { get; set; }
    public string Type => TYPE;
}
