using FormBuilder.Rules;

namespace FormBuilder.Models.Rules;

public class IncomingTokensRepetitionRule : IRepetitionRule
{
    public const string TYPE = "INCOMINGTOKENS";
    public string Path { get; set; }
    public bool MapSameTranslationToAllSupportedLanguages { get; set; } = false;
    public List<MappingRule> FormRecordProperties { get; set; } = new List<MappingRule>();
    public List<MappingRule> AdditionalInputTokensComingFromStepSource { get; set; } = new List<MappingRule>();
    public List<LabelMappingRule> LabelMappingRules { get; set; } = new List<LabelMappingRule>();
    public string Type => TYPE;
}