﻿using FormBuilder.Models;
using FormBuilder.Models.Rules;
using System.Text.Json.Nodes;

namespace FormBuilder.Rules;

public abstract class GenericRepetitionRuleEngine<T> : IRepetitionRuleEngine where T : IRepetitionRule
{
    public abstract string Type { get; }

    public List<IFormElementRecord> Transform(string fieldType, JsonObject input, IRepetitionRule parameter, Dictionary<string, object> parameters) => InternalTransform(fieldType, input, (T)parameter, parameters);

    protected abstract List<IFormElementRecord> InternalTransform(string fieldType, JsonObject input, T parameter, Dictionary<string, object> parameters);
}
