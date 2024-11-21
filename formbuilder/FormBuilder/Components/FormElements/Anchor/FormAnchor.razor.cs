using FormBuilder.Components.Drag;
using FormBuilder.Factories;
using FormBuilder.Url;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FormBuilder.Components.FormElements.Anchor;

public partial class FormAnchor : IGenericFormElement<FormAnchorRecord>
{
    ElementReference LinkElt;
    string Url { get; set; }
    [Parameter] public FormAnchorRecord Value { get; set; }
    [Parameter] public FormViewerContext Context { get; set; }
    [Parameter] public bool IsEditModeEnabled { get; set; }
    [Parameter] public ParentEltContext ParentContext {  get; set; }
    [Inject] private ITargetUrlHelperFactory targetUrlHelperFactory { get; set; }
    [Inject] private IJSRuntime JSRuntime { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if(Value != null && Value.Url != null)
        {
            var builder = targetUrlHelperFactory.Build(Value.Url.Type);
            Url = builder.Build(Value.Url);
        }
    }

    async Task Navigate()
    {
        if (string.IsNullOrWhiteSpace(Url)) return; 
        await JSRuntime.InvokeVoidAsync("FormBuilder.navigate", LinkElt);
    }
}