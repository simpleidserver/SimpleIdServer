using FormBuilder.Components.Drag;
using FormBuilder.Factories;
using FormBuilder.Helpers;
using Microsoft.AspNetCore.Components;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.StackLayout;

public partial class FormStackLayout : IGenericFormElement<FormStackLayoutRecord>
{
    private RenderFragment? CustomRender { get; set; }
    [Inject] private IRenderFormElementsHelper renderFormsElementsHelper {  get; set; }
    [Inject] private IHttpClientFactory httpClientFactory { get; set; }
    [Inject] private ITargetUrlHelperFactory targetUrlHelperFactory { get; set; }
    [Inject] private IUriProvider uriProvider {  get; set; }
    [Inject] private IServiceProvider serviceProvider { get; set; }
    [Inject] private IWorkflowLinkActionFactory WorkflowLinkActionFactory { get; set; }
    [Parameter] public WorkflowExecutionContext WorkflowExecutionContext { get; set; }
    [Parameter] public FormStackLayoutRecord Value { get; set; }
    [Parameter] public FormViewerContext Context { get; set; }
    [Parameter] public bool IsEditModeEnabled { get; set; }
    [Parameter] public ParentEltContext ParentContext { get; set; }
    [Parameter] public WorkflowViewerContext WorkflowContext { get; set; }
    [Parameter] public bool IsInteractableElementEnabled { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (Value != null)
        {
            Value.Elements.CollectionChanged += HandleCollectionChanged;
            CustomRender = CreateComponent();
        }
    }

    async Task Submit()
    {
        var link = WorkflowExecutionContext.GetLink(Value);
        if (link == null) return;
        var json = new JsonObject();
        Value.ExtractJson(json);
        WorkflowExecutionContext.SetStepOutput(json);
        var act = WorkflowLinkActionFactory.Build(link.ActionType);
        await act.Execute(link, WorkflowExecutionContext);
    }

    private RenderFragment CreateComponent() => builder =>
    {
        renderFormsElementsHelper.Render(builder, Value.Elements, Context, IsEditModeEnabled, WorkflowContext, IsInteractableElementEnabled, WorkflowExecutionContext);
    };

    private void HandleCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        CustomRender = CreateComponent();
        StateHasChanged();
    }

    private Dictionary<string, string> ConvertToDic(JsonObject json)
    {
        var result = new Dictionary<string, string>();
        foreach (var kvp in json)
        {
            result.Add(kvp.Key, json[kvp.Key].ToString());
        }

        return result;
    }
}
