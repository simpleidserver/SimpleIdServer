using FormBuilder.Components;
using FormBuilder.Factories;
using FormBuilder.Link.Components;
using FormBuilder.Models;
using FormBuilder.Models.Url;
using FormBuilder.Rules;
using FormBuilder.Services;
using FormBuilder.Transformers;
using Json.Path;
using Microsoft.AspNetCore.Components.Rendering;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace FormBuilder.Link;

public class WorkflowLinkUrlTransformerAction : IWorkflowLinkAction
{
    private readonly IUrlEvaluatorFactory _urlEvaluatorFactory;
    private readonly IFormBuilderJsService _formBuilderJsService;

    public WorkflowLinkUrlTransformerAction(IUrlEvaluatorFactory urlEvaluatorFactory, IFormBuilderJsService formBuilderJsService)
    {
        _urlEvaluatorFactory = urlEvaluatorFactory;
        _formBuilderJsService = formBuilderJsService;
    }

    public static string ActionType => "UrlTransformation";

    public string Type => ActionType;

    public string DisplayName => "Url transformation";

    public List<string> ExcludedStepNames => new List<string>();

    public bool CanBeAppliedMultipleTimes => true;

    public async Task Execute(WorkflowLink activeLink, WorkflowExecutionContext context)
    {
        if (string.IsNullOrWhiteSpace(activeLink.ActionParameter) || context.RepetitionRuleData == null) return;
        var parameter = JsonSerializer.Deserialize<WorkflowLinkUrlTransformationParameter>(activeLink.ActionParameter);
        if (string.IsNullOrWhiteSpace(parameter.JsonSource)) return;
        var path = JsonPath.Parse(parameter.JsonSource);
        var pathResult = path.Evaluate(context.RepetitionRuleData);
        var nodes = pathResult.Matches.Select(m => m.Value);
        if (nodes.Count() != 1) return;
        var value = nodes.Single()?.ToString();
        var url = _urlEvaluatorFactory.Evaluate(new DirectTargetUrl
        {
            Parameters = new Dictionary<string, string>
            {
                { parameter.QueryParameterName, value }
            },
            Url = parameter.Url
        });
        await _formBuilderJsService.NavigateForce(url);
    }

    public object Render(RenderTreeBuilder builder, WorkflowLink workflowLink)
    {
        var parameter = new WorkflowLinkUrlTransformationParameter();
        if(!string.IsNullOrWhiteSpace(workflowLink.ActionParameter))
            parameter = JsonSerializer.Deserialize<WorkflowLinkUrlTransformationParameter>(workflowLink.ActionParameter);

        builder.OpenComponent<WorkflowLinkUrlTransformerComponent>(0);
        builder.AddAttribute(1, nameof(WorkflowLinkUrlTransformerComponent.Parameter), parameter);
        builder.CloseComponent();
        return parameter;
    }
}
