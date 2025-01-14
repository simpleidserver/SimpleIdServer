using FormBuilder.Components;
using FormBuilder.Factories;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Rules.Components;
using Json.Path;
using Microsoft.AspNetCore.Components.Rendering;
using System.Reflection;
using System.Text.Json.Nodes;

namespace FormBuilder.Rules;

public class IncomingTokensRepetitionRuleEngine : GenericRepetitionRuleEngine<IncomingTokensRepetitionRule>
{
    private readonly ITransformerFactory _transformerFactory;

    public IncomingTokensRepetitionRuleEngine(ITransformerFactory transformerFactory)
    {
        _transformerFactory = transformerFactory;
    }

    public override string Type => IncomingTokensRepetitionRule.TYPE;

    protected override List<(IFormElementRecord, JsonNode)> InternalTransform(List<IFormElementDefinition> definitions, string fieldType, JsonObject input, IncomingTokensRepetitionRule parameter, Dictionary<string, object> parameters)
    {
        var result = new List<(IFormElementRecord, JsonNode)>();
        var path = JsonPath.Parse(parameter.Path);
        var pathResult = path.Evaluate(input);
        var nodes = pathResult.Matches.Select(m => m.Value);
        var definition = definitions.Single(d => d.Type == fieldType);
        var recordType = definition.RecordType;
        var allMappingRuleSourceLst = parameter.MappingRules.Select(r => r.Target).Distinct();
        var allProperties = recordType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        var filteredProperties = allProperties
            .Where(p => allMappingRuleSourceLst.Contains(p.Name))
            .ToList();
        foreach (var node in nodes)
        {
            var instance = Activator.CreateInstance(recordType);
            if (parameter.MappingRules != null)
                foreach(var mappingRule in parameter.MappingRules)
                    ApplyProperty(instance, mappingRule, node, filteredProperties);

            var formField = instance as BaseFormFieldRecord;
            if(formField != null && parameter.LabelMappingRules != null)
            {
                formField.Labels = new List<LabelTranslation>();
                foreach (var labelMappingRule in parameter.LabelMappingRules)
                    ApplyLabel(formField, node, labelMappingRule);
            }

            var formElt = instance as IFormElementRecord;
            if(formElt != null)
                formElt.Id = Guid.NewGuid().ToString();

            ApplyParameters(instance, allProperties, parameters);
            var record = (IFormElementRecord)instance;
            result.Add((record, node));
        }

        return result;
    }
    
    protected override void InternalBuildComponent(WorkflowContext context, IncomingTokensRepetitionRule target, Type recordType, RenderTreeBuilder builder)
    {
        builder.OpenComponent<IncomingTokensRepetitionRuleComponent>(0);
        builder.AddAttribute(1, nameof(IncomingTokensRepetitionRuleComponent.Record), target);
        builder.AddAttribute(2, nameof(IncomingTokensRepetitionRuleComponent.RecordType), recordType);
        builder.AddAttribute(3, nameof(IncomingTokensRepetitionRuleComponent.Context), context);
        builder.CloseComponent();
    }

    private void ApplyProperty(object instance, MappingRule mappingRule, JsonNode node, List<PropertyInfo> properties)
    {
        if (mappingRule.Source == null) return;
        var path = JsonPath.Parse(mappingRule.Source);
        var pathResult = path.Evaluate(node);
        var nodes = pathResult.Matches.Select(m => m.Value);
        if (nodes.Count() != 1) return;
        object value = nodes.Single();
        if (mappingRule.Transformer != null) value = _transformerFactory.Transform(value?.ToString(), mappingRule.Transformer, node);
        var definitionProperty = properties.SingleOrDefault(p => p.Name == mappingRule.Target);
        if (definitionProperty == null) return;
        definitionProperty.SetValue(instance, value);
    }

    private void ApplyLabel(BaseFormFieldRecord instance, JsonNode node, LabelMappingRule labelMappingRule)
    {
        var path = JsonPath.Parse(labelMappingRule.Source);
        var pathResult = path.Evaluate(node);
        var nodes = pathResult.Matches.Select(m => m.Value);
        if (nodes.Count() != 1) return;
        var translation = nodes.Single()?.ToString();
        instance.Labels.Add(new LabelTranslation { Language = labelMappingRule.Language, Translation = translation, ConditionParameter = null });
    }

    private void ApplyParameters(object instance, IEnumerable<PropertyInfo> properties, Dictionary<string, object> parameters)
    {
        if (parameters == null) return;
        foreach(var kvp in parameters)
        {
            var propertyInfo = properties.SingleOrDefault(p => p.Name == kvp.Key);
            if (propertyInfo == null) continue;
            if(propertyInfo.PropertyType == typeof(bool)) propertyInfo.SetValue(instance, bool.Parse(kvp.Value.ToString()));
            else propertyInfo.SetValue(instance, kvp.Value);
        }
    }
}
