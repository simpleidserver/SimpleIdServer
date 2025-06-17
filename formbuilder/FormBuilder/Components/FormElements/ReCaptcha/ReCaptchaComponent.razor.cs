using FormBuilder.Components.Drag;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.ComponentModel;

namespace FormBuilder.Components.FormElements.ReCaptcha
{
    public partial class ReCaptchaComponent : IGenericFormElement<ReCaptchaRecord>
    {
        [Parameter]
        public ReCaptchaRecord Value
        {
            get; set;
        }

        [Parameter]
        public ParentEltContext ParentContext
        {
            get; set;
        }

        [Parameter]
        public WorkflowContext Context
        {
            get; set;
        }

        [Parameter]
        public bool IsEditModeEnabled
        {
            get; set;
        }

        [Inject]
        private IJSRuntime jSRuntime
        {
            get; set;
        }

        private string UniqueId = Guid.NewGuid().ToString();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                if (!IsEditModeEnabled && !string.IsNullOrWhiteSpace(Value.SiteKey))
                {
                    await jSRuntime.InvokeAsync<object>("reCaptchaV2.init");
                    await jSRuntime.InvokeAsync<int>("reCaptchaV2.render", DotNetObjectReference.Create(this), UniqueId, Value.SiteKey);
                }
            }
        }


        [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
        public void CallbackOnSuccess(string response)
        {
            Value.Value = response;
            StateHasChanged();
        }

        [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
        public void CallbackOnExpired()
        {

        }
    }
}
