using FormBuilder.Models.Rules;
using FormBuilder.Models.Transformer;
using FormBuilder.Rules;
using FormBuilder.Transformers.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Collections.ObjectModel;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace FormBuilder.Transformers;

public class RegexTransformerParameters : ITransformerParameters
{
    public string Type => TYPE;
    public const string TYPE = "RegexTransformer";
    public ObservableCollection<MappingRule> Rules { get; set; } = new ObservableCollection<MappingRule>();
}

public class RegexTransformer : GenericTransformer<RegexTransformerParameters>
{
    private readonly IMappingRuleService _mappingRuleService;

    public RegexTransformer(IMappingRuleService mappingRuleService)
    {
        _mappingRuleService = mappingRuleService;
    }

    public override string Type => RegexTransformerParameters.TYPE;

    public override ITransformerParameters CreateEmptyInstance()
    {
        return new RegexTransformerParameters();
    }

    internal override object InternalTransform(string value, RegexTransformerParameters parameters, JsonNode data)
    {
        var result = value;
        var json = _mappingRuleService.Extract(data.AsObject(), parameters.Rules);
        var matches = Regex.Matches(value, @"{\w+}");
        foreach (Match match in matches)
        {
            var matchValue = match.Value;
            var key = matchValue
                .Replace("{", "")
                .Replace("}", "");
            if(json.ContainsKey(key))
                result = result.Replace(matchValue, json[key].ToString());
        }

        if(!Uri.TryCreate(result, UriKind.RelativeOrAbsolute, out var Url))
        {
            result = result.Replace("//", "/");
        }

        return result;
    }

    internal override void InternalBuild(RegexTransformerParameters parameters, RenderTreeBuilder builder)
    {
        builder.OpenComponent<RegexTransformerComponent>(0);
        builder.AddAttribute(1, nameof(RegexTransformerComponent.Record), parameters);
        builder.CloseComponent();
    }
}