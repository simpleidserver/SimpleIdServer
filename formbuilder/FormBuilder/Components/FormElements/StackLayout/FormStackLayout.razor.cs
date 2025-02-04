using FormBuilder.Components.Drag;
using FormBuilder.Factories;
using FormBuilder.Helpers;
using FormBuilder.UIs;
using Microsoft.AspNetCore.Components;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.StackLayout;

public partial class FormStackLayout : IGenericFormElement<FormStackLayoutRecord>
{
    private RenderFragment? CustomRender { get; set; }
    [Inject] private IRenderFormElementsHelper renderFormsElementsHelper {  get; set; }
    [Inject] private IWorkflowLinkActionFactory WorkflowLinkActionFactory { get; set; }
    [Parameter] public FormStackLayoutRecord Value { get; set; }
    [Parameter] public WorkflowContext Context { get; set; }
    [Parameter] public bool IsEditModeEnabled { get; set; }
    [Parameter] public ParentEltContext ParentContext { get; set; }
    [Parameter] public bool IsInteractableElementEnabled { get; set; }
    public string TargetUrl { get; private set; }
    public Dictionary<string, string> HiddenFields { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (Value != null)
        {
            Value.Elements.CollectionChanged += HandleCollectionChanged;
            CustomRender = CreateComponent();
            BuildHiddenFields();
        }
    }

    private async Task Submit()
    {
        var link = Context.GetLinkDefinitionFromCurrentStep(Value.Id);
        if (link == null || Value.IsSubmitting) return;
        Value.Submit();
        var json = new JsonObject();
        Value.ExtractJson(json);
        var execution = Context.GetLinkExecutionFromElementAndCurrentStep(Value.Id);
        execution.OutputData = json;
        var act = WorkflowLinkActionFactory.Build(link.ActionType);
        await act.Execute(link, execution, Context);
        Value.FinishSubmit();
    }

    private RenderFragment CreateComponent() => builder =>
    {
        renderFormsElementsHelper.Render(builder, Value.Elements, Context, IsEditModeEnabled, IsInteractableElementEnabled);
    };

    private void HandleCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        CustomRender = CreateComponent();
        StateHasChanged();
    }

    private void BuildHiddenFields()
    {
        if (Value.FormType != FormTypes.HTML || !string.IsNullOrWhiteSpace(TargetUrl)) return;
        HiddenFields = new Dictionary<string, string>();
        var link = Context.GetLinkDefinitionFromCurrentStep(Value.Id);
        if (link == null) return;
        var json = new JsonObject();
        Value.ExtractJson(json);
        var execution = Context.GetLinkExecutionFromElementAndCurrentStep(Value.Id);
        execution.OutputData = json;
        var act = WorkflowLinkActionFactory.Build(link.ActionType);
        var res = act.GetRequest(link, execution, Context);
        if (res == null) return;
        TargetUrl = res.Value.url;
        if (Context.Execution?.AntiforgeryToken != null && res.Value.json.ContainsKey(Context.Execution.AntiforgeryToken.FormField))
            HiddenFields.Add(Context.Execution.AntiforgeryToken.FormField, Context.Execution.AntiforgeryToken.FormValue);

        if(res.Value.json.ContainsKey(nameof(IStepViewModel.StepId))) HiddenFields.Add(nameof(IStepViewModel.StepId), res.Value.json[nameof(IStepViewModel.StepId)].ToString());
        if (res.Value.json.ContainsKey(nameof(IStepViewModel.WorkflowId))) HiddenFields.Add(nameof(IStepViewModel.WorkflowId), res.Value.json[nameof(IStepViewModel.WorkflowId)].ToString());
        if (res.Value.json.ContainsKey(nameof(IStepViewModel.CurrentLink))) HiddenFields.Add(nameof(IStepViewModel.CurrentLink), res.Value.json[nameof(IStepViewModel.CurrentLink)].ToString());
    }
}
