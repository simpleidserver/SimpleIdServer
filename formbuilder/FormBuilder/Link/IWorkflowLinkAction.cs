using FormBuilder.Components;
using FormBuilder.Models;
using Microsoft.AspNetCore.Components.Rendering;

namespace FormBuilder.Link;

public interface IWorkflowLinkAction
{
    string Type { get; }
    string DisplayName { get; }
    List<string> ExcludedStepNames { get; }
    bool CanBeAppliedMultipleTimes { get; }
    Task Execute(WorkflowLink activeLink, WorkflowExecutionContext context);
    void Render(RenderTreeBuilder builder, WorkflowLink workflowLink);
}