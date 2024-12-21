﻿using FormBuilder.Models;
using FormBuilder.Models.Rules;
using Microsoft.AspNetCore.Components.Rendering;
using System.Text.Json.Nodes;

namespace FormBuilder.Rules;

public abstract class GenericRepetitionRuleEngine<T> : IRepetitionRuleEngine where T : IRepetitionRule
{
    public abstract string Type { get; }

    public List<(IFormElementRecord, JsonNode)> Transform(List<IFormElementDefinition> definitions, string fieldType, JsonObject input, IRepetitionRule parameter, Dictionary<string, object> parameters) => InternalTransform(definitions, fieldType, input, (T)parameter, parameters);

    public void BuildComponent(IRepetitionRule target, Type recordType, RenderTreeBuilder builder) => InternalBuildComponent((T)target, recordType, builder);

    protected abstract List<(IFormElementRecord, JsonNode)> InternalTransform(List<IFormElementDefinition> definitions, string fieldType, JsonObject input, T parameter, Dictionary<string, object> parameters);

    protected abstract void InternalBuildComponent(T target, Type recordType, RenderTreeBuilder builder);
}