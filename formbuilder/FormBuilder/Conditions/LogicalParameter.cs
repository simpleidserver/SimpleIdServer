namespace FormBuilder.Conditions;

public class LogicalParameter : IConditionParameter
{
    public static string TYPE = "LogicalParameter";
    public string Type => TYPE;
    public IConditionParameter LeftExpression { get; set; }
    public IConditionParameter RightExpression { get; set; }
    public LogicalOperators Operator { get; set; }
}
