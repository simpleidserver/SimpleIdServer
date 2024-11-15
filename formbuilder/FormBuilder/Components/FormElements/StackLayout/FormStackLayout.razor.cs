using FormBuilder.Factories;
using FormBuilder.Helpers;
using Microsoft.AspNetCore.Components;
using System.Text;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.StackLayout;

public partial class FormStackLayout : IGenericFormElement<FormStackLayoutRecord>
{
    private RenderFragment? CustomRender { get; set; }
    [Inject] private IRenderFormElementsHelper renderFormsElementsHelper {  get; set; }
    [Inject] private IHttpClientFactory httpClientFactory { get; set; }
    [Inject] private ITargetUrlHelperFactory targetUrlHelperFactory { get; set; }
    [Inject] private IUriProvider uriProvider {  get; set; }
    [Parameter] public FormStackLayoutRecord Value { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (Value != null)
        {
            CustomRender = CreateComponent();
        }
    }

    async Task Submit()
    {
        if (Value.Url == null) return;
        var json = new JsonObject();
        Value.ExtractJson(json);
        var targetUrl = targetUrlHelperFactory.Build(Value.Url);
        using (var httpClient = httpClientFactory.CreateClient())
        {
            var url = new Uri($"{uriProvider.GetAbsoluteUriWithVirtualPath()}{targetUrl}");
            var requestMessage = new HttpRequestMessage
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "Login", "Login" }
                }),
                Method = HttpMethod.Post,
                RequestUri = url
            };
            var httpResult  = await httpClient.SendAsync(requestMessage);
        }
    }

    private RenderFragment CreateComponent() => builder =>
    {
        renderFormsElementsHelper.Render(builder, Value.Elements);
    };
}
