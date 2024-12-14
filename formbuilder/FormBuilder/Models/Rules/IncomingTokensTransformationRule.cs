namespace FormBuilder.Models.Rules;

public class IncomingTokensTransformationRule : ITransformationRule
{
    public string Source { get; set; }
    public string Type => TYPE;
    public static string TYPE = "IncomingTokensTransRule";
}