using FormBuilder.Components;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using Microsoft.AspNetCore.Components.Rendering;
using System.Text.Json.Nodes;

namespace FormBuilder.Rules;

public interface IRepetitionRuleEngine
{
    string Type { get; }
    List<(IFormElementRecord, JsonNode)> Transform(List<IFormElementDefinition> definitions, string fieldType, JsonObject input, IRepetitionRule parameter, Dictionary<string, object> parameters);
    void BuildComponent(WorkflowContext context, IRepetitionRule target, Type recordType, RenderTreeBuilder builder);
}
