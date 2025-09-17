using FormBuilder.Components.Drag;
using FormBuilder.Factories;
using FormBuilder.Helpers;
using FormBuilder.Services;
using Microsoft.AspNetCore.Components;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.Anchor;

public partial class FormAnchor : IGenericFormElement<FormAnchorRecord>
{
    [Parameter] public ParentEltContext ParentContext { get; set; }
    [Parameter] public FormAnchorRecord Value { get; set; }
    [Parameter] public bool IsEditModeEnabled { get; set; }
    [Parameter] public WorkflowContext Context { get; set; }
    public string CssId
    {
        get
        {
            return Value.CssId ?? string.Empty;
        }
    }
    [Inject] private IWorkflowLinkActionFactory WorkflowLinkActionFactory { get; set; }
    [Inject] private IHtmlClassResolver htmlClassResolver { get; set; }
    [Inject] private IFormBuilderJsService formBuilderJsService { get; set; }
    public JsonNode InputData
    {
        get
        {
            var linkExecution = Context.GetCurrentStepExecution();
            return linkExecution?.InputData;
        }
    }

    public string BtnClass
    {
        get
        {
            return htmlClassResolver.Resolve(Value, AnchorElementNames.Btn, Context);
        }
    }

    public string AnchorClass
    {
        get
        {
            return htmlClassResolver.Resolve(Value, AnchorElementNames.Anchor, Context);
        }
    }

    private async Task Navigate()
    {
        if(Value.IsStaticLink)
        {
            if(Value.OpenInNewTab)
            {
                await formBuilderJsService.Navigate(Value.Url);
            }
            else
            {
                await formBuilderJsService.NavigateForce(Value.Url);
            }
        }
        else
        {
            var linkExecution = Context.GetLinkExecutionFromElementAndCurrentStep(Value.Id);
            var link = Context.GetLinkDefinitionFromCurrentStep(Value.Id);
            if (linkExecution == null || link == null) return;
            var act = WorkflowLinkActionFactory.Build(link.ActionType);
            await act.Execute(link, linkExecution, Context);
        }
    }
}