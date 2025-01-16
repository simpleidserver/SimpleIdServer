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

    protected override List<(IFormElementRecord, JsonNode)> InternalTransform(List<string> supportedLanguageCodes, List<IFormElementDefinition> definitions, string fieldType, JsonObject input, IncomingTokensRepetitionRule parameter, Dictionary<string, object> parameters)
    {
        var result = new List<(IFormElementRecord, JsonNode)>();
        var path = JsonPath.Parse(parameter.Path);
        var pathResult = path.Evaluate(input);
        var nodes = pathResult.Matches.Select(m => m.Value).Select(m => m.AsObject());
        var definition = definitions.Single(d => d.Type == fieldType);
        var recordType = definition.RecordType;
        var allMappingRuleSourceLst = parameter.FormRecordProperties.Select(r => r.Target).Distinct();
        var allProperties = recordType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        var filteredProperties = allProperties
            .Where(p => allMappingRuleSourceLst.Contains(p.Name))
            .ToList();
        foreach (var node in nodes)
        {
            var instance = Activator.CreateInstance(recordType);
            // Retrieve tokens from the step and add them to the input.
            if (parameter.AdditionalInputTokensComingFromStepSource != null)
                foreach (var mapping in parameter.AdditionalInputTokensComingFromStepSource)
                    AddToken(input, node, mapping);

            // Update the properties of the form record.
            if (parameter.FormRecordProperties != null)
                foreach(var mappingRule in parameter.FormRecordProperties)
                    ApplyProperty(instance, mappingRule, node, filteredProperties);

            // Update the labels of the form record.
            var formField = instance as BaseFormFieldRecord;
            if(formField != null && parameter.LabelMappingRules != null)
            {
                formField.Labels = new List<LabelTranslation>();
                if (parameter.MapSameTranslationToAllSupportedLanguages)
                    foreach (var grp in parameter.LabelMappingRules.GroupBy(r => r.Source))
                        ApplyLabel(formField, node, grp.Key, supportedLanguageCodes);
                else
                    foreach (var labelMappingRule in parameter.LabelMappingRules)
                        ApplyLabel(formField, node, labelMappingRule.Source, new List<string> { labelMappingRule.Language });
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

    private void AddToken(JsonObject stepData, JsonObject eltData, MappingRule mappingRule)
    {
        if (mappingRule.Source == null) return;
        var path = JsonPath.Parse(mappingRule.Source);
        var pathResult = path.Evaluate(stepData);
        var nodes = pathResult.Matches.Select(m => m.Value);
        if (nodes.Count() != 1 || eltData.ContainsKey(mappingRule.Target)) return;
        eltData.Add(mappingRule.Target, nodes.First().ToString());
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

    private void ApplyLabel(BaseFormFieldRecord instance, JsonNode node, string source, List<string> languages)
    {
        var path = JsonPath.Parse(source);
        var pathResult = path.Evaluate(node);
        var nodes = pathResult.Matches.Select(m => m.Value);
        if (nodes.Count() != 1) return;
        var translation = nodes.Single()?.ToString();
        foreach(var language in languages)
            instance.Labels.Add(new LabelTranslation { Language = language, Translation = translation, ConditionParameter = null });
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
