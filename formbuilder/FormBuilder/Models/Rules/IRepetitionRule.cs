﻿using System.Text.Json.Serialization;

namespace FormBuilder.Models.Rules;

[JsonDerivedType(typeof(IncomingTokensRepetitionRule), typeDiscriminator: "IncomingTokensRepRule")]
public interface IRepetitionRule
{
    string Path { get; }
    string Type { get; }
    string FieldType { get; set; }
    List<MappingRule> MappingRules { get; set; }
}