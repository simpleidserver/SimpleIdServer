using FormBuilder.Components;
using FormBuilder.Link.Components;
using FormBuilder.Models;
using FormBuilder.Services;
using Microsoft.AspNetCore.Components.Rendering;
using System.Text.Json;

namespace FormBuilder.Link;

public class WorkflowLinkUrlAction : IWorkflowLinkAction
{
    private readonly IFormBuilderJsService _formBuidlerJsService;

    public WorkflowLinkUrlAction(IFormBuilderJsService formBuilderJsService)
    {
        _formBuidlerJsService = formBuilderJsService;
    }

    public string Type => ActionType;

    public string DisplayName => "URL";

    public static string ActionType => "URL";

    public List<string> ExcludedStepNames => new List<string>();

    public bool CanBeAppliedMultipleTimes => false;

    public async Task Execute(WorkflowLink activeLink, WorkflowExecutionContext context)
    {
        if(string.IsNullOrWhiteSpace(activeLink.ActionParameter)) return;
        var parameter = JsonSerializer.Deserialize<WorkflowLinkUrlParameter>(activeLink.ActionParameter);
        await _formBuidlerJsService.Navigate(parameter.Url);
    }

    public void Render(RenderTreeBuilder builder, WorkflowLink workflowLink)
    {
        var parameter = new WorkflowLinkUrlParameter();
        if(!string.IsNullOrWhiteSpace(workflowLink.ActionParameter))
            parameter = JsonSerializer.Deserialize<WorkflowLinkUrlParameter>(workflowLink.ActionParameter);
        builder.OpenComponent<EditWorkflowLinkComponent>(0);
        builder.AddAttribute(1, nameof(EditWorkflowLinkComponent.Parameter), parameter);
        builder.AddAttribute(2, nameof(EditWorkflowLinkComponent.WorkflowLink), workflowLink);
        builder.CloseComponent();
    }
}
