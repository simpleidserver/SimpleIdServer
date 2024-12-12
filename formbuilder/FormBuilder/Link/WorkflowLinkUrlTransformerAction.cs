using FormBuilder.Components;
using FormBuilder.Link.Components;
using FormBuilder.Models;
using Microsoft.AspNetCore.Components.Rendering;
using System.Text.Json;

namespace FormBuilder.Link;

public class WorkflowLinkUrlTransformerAction : IWorkflowLinkAction
{
    public static string ActionType => "UrlTransformation";

    public string Type => ActionType;

    public string DisplayName => "Url transformation";

    public List<string> ExcludedStepNames => new List<string>();

    public bool CanBeAppliedMultipleTimes => true;

    public Task Execute(WorkflowLink activeLink, WorkflowExecutionContext context)
    {
        return Task.CompletedTask;
    }

    public void Render(RenderTreeBuilder builder, WorkflowLink workflowLink)
    {
        var parameter = new WorkflowLinkUrlTransformationParameter();
        if(!string.IsNullOrWhiteSpace(workflowLink.ActionParameter))
            parameter = JsonSerializer.Deserialize<WorkflowLinkUrlTransformationParameter>(workflowLink.ActionParameter);

        builder.OpenComponent<WorkflowLinkUrlTransformerComponent>(0);
        builder.AddAttribute(1, nameof(WorkflowLinkUrlTransformerComponent.Parameter), parameter);
        builder.AddAttribute(2, nameof(WorkflowLinkUrlTransformerComponent.WorkflowLink), workflowLink);
        builder.CloseComponent();
    }
}
