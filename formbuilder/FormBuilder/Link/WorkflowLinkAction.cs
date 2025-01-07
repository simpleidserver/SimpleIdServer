using FormBuilder.Components;
using FormBuilder.Link.Components;
using FormBuilder.Models;
using Microsoft.AspNetCore.Components.Rendering;
using System.Text.Json.Nodes;

namespace FormBuilder.Link;

public class WorkflowLinkAction : IWorkflowLinkAction
{
    public static string ActionType => "LINK";
    public string Type => ActionType;
    public string DisplayName => "Link";

    public List<string> ExcludedStepNames => new List<string>();

    public bool CanBeAppliedMultipleTimes => false;

    public Task Execute(WorkflowLink activeLink, WorkflowStepLinkExecution linkExecution, WorkflowContext context)
    {
        context.NextStep(activeLink);
        return Task.CompletedTask;
    }

    public (JsonObject json, string url)? GetRequest(WorkflowLink activeLink, WorkflowStepLinkExecution linkExecution, WorkflowContext context)
    {
        return null;
    }

    public object Render(RenderTreeBuilder builder, WorkflowLink workflowLink, JsonNode fakeData, WorkflowContext context)
    {
        builder.OpenComponent<WorkflowLinkComponent>(0);
        builder.CloseComponent();
        return new WorkflowLinkParameter();
    }
}
