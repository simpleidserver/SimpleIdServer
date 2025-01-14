using FormBuilder.Components.Drag;
using FormBuilder.Factories;
using Microsoft.AspNetCore.Components;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.Anchor;

public partial class FormAnchor : IGenericFormElement<FormAnchorRecord>
{
    [Parameter] public ParentEltContext ParentContext { get; set; }
    [Parameter] public FormAnchorRecord Value { get; set; }
    [Parameter] public bool IsEditModeEnabled { get; set; }
    [Parameter] public bool IsInteractableElementEnabled { get; set; }
    [Parameter] public WorkflowContext Context { get; set; }
    [Inject] private IWorkflowLinkActionFactory WorkflowLinkActionFactory { get; set; }
    public JsonNode InputData
    {
        get
        {
            var linkExecution = Context.GetCurrentStepExecution();
            return linkExecution?.InputData;
        }
    }

    private async Task Navigate()
    {
        var linkExecution = Context.GetLinkExecutionFromElementAndCurrentStep(Value.Id);
        var link = Context.GetLinkDefinitionFromCurrentStep(Value.Id);
        if (linkExecution == null || link == null) return;
        var act = WorkflowLinkActionFactory.Build(link.ActionType);
        await act.Execute(link, linkExecution, Context);
    }
}