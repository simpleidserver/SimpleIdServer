using FormBuilder.Rules;

namespace FormBuilder.Models.Rules;

public class IncomingTokensRepetitionRule : IRepetitionRule
{
    public const string TYPE = "INCOMINGTOKENS";
    public string Path { get; set; }
    public List<MappingRule> MappingRules { get; set; }
    public List<LabelMappingRule> LabelMappingRules { get; set; }
    public string Type => TYPE;
}
