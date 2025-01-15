using FormBuilder.Components;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using Microsoft.AspNetCore.Components.Rendering;
using System.Text.Json.Nodes;

namespace FormBuilder.Rules;

public abstract class GenericRepetitionRuleEngine<T> : IRepetitionRuleEngine where T : IRepetitionRule
{
    public abstract string Type { get; }

    public List<(IFormElementRecord, JsonNode)> Transform(List<string> supportedLanguageCodes, List<IFormElementDefinition> definitions, string fieldType, JsonObject input, IRepetitionRule parameter, Dictionary<string, object> parameters) => InternalTransform(supportedLanguageCodes, definitions, fieldType, input, (T)parameter, parameters);

    public void BuildComponent(WorkflowContext context, IRepetitionRule target, Type recordType, RenderTreeBuilder builder) => InternalBuildComponent(context, (T)target, recordType, builder);

    protected abstract List<(IFormElementRecord, JsonNode)> InternalTransform(List<string> supportedLanguageCodes, List<IFormElementDefinition> definitions, string fieldType, JsonObject input, T parameter, Dictionary<string, object> parameters);

    protected abstract void InternalBuildComponent(WorkflowContext context, T target, Type recordType, RenderTreeBuilder builder);
}
