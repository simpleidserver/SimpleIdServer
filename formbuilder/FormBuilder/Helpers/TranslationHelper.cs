using FormBuilder.Factories;
using FormBuilder.Models;
using System.Text.Json.Nodes;

namespace FormBuilder.Helpers;

public interface ITranslationHelper
{
    string Translate(IFormElementRecord record, JsonNode inputData, bool executeCondition);
    string Translate(IFormElementRecord record, string defaultText, JsonNode inputData, bool executeCondition);
}

public class TranslationHelper : ITranslationHelper
{
    private readonly IConditionRuleEngineFactory _conditionRuleEngineFactory;

    public TranslationHelper(IConditionRuleEngineFactory conditionRuleEngineFactory)
    {
        _conditionRuleEngineFactory = conditionRuleEngineFactory;
    }

    public string Translate(IFormElementRecord record, JsonNode inputData, bool executeCondition)
    {
        if (record.Labels == null || !record.Labels.Any()) return string.Empty;
        var currentCulture = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
        var filteredLabels = record.Labels.Where(l => l.Language == currentCulture);
        var filteredLabelsWithConditions = filteredLabels.Where(l => l.ConditionParameter != null);
        if(inputData != null && inputData.GetValueKind() == System.Text.Json.JsonValueKind.Object)
        {
            var obj = inputData.AsObject();
            foreach (var label in filteredLabelsWithConditions)
                if (_conditionRuleEngineFactory.Evaluate(obj, label.ConditionParameter)) return label.Translation;
        }

        return filteredLabels.FirstOrDefault(l => l.ConditionParameter == null)?.Translation;
    }

    public string Translate(IFormElementRecord record, string defaultText, JsonNode inputData, bool executeCondition)
    {
        var result = Translate(record, inputData, executeCondition);
        return string.IsNullOrWhiteSpace(result) ? defaultText : result;
    }
}