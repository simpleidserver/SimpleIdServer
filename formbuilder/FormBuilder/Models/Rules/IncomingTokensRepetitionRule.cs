﻿using FormBuilder.Rules;

namespace FormBuilder.Models.Rules;

public class IncomingTokensRepetitionRule : IRepetitionRule
{
    public const string TYPE = "INCOMINGTOKENS";
    public string Path { get; set; }
    public List<MappingRule> MappingRules { get; set; } = new List<MappingRule>();
    public List<LabelMappingRule> LabelMappingRules { get; set; } = new List<LabelMappingRule>();
    public List<LinkWorkflowMappingRule> LinkWorkflowMappingRules { get; set; } = new List<LinkWorkflowMappingRule>();
    public string Type => TYPE;
}