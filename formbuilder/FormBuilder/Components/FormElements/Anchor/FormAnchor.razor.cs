using FormBuilder.Url;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FormBuilder.Components.FormElements.Anchor;

public partial class FormAnchor : IGenericFormElement<FormAnchorRecord>
{
    ElementReference LinkElt;
    [Parameter] public FormAnchorRecord Value { get; set; }
    [Parameter] public AntiforgeryTokenRecord AntiforgeryToken { get; set; }
    [Inject] private ITargetUrlHelper targetUrlHelper { get; set; }
    [Inject] private IJSRuntime JSRuntime { get; set; }

    async Task Navigate()
    {
        var url = targetUrlHelper.Build(Value.Url);
        if (string.IsNullOrWhiteSpace(url)) return; 
        await JSRuntime.InvokeVoidAsync("FormBuilder.navigate", LinkElt);
    }
}