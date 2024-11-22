using FormBuilder.Components.Drag;
using FormBuilder.Factories;
using FormBuilder.Helpers;
using Microsoft.AspNetCore.Components;
using System.Net;
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
    [Parameter] public FormStackLayoutRecord Value { get; set; }
    [Parameter] public FormViewerContext Context { get; set; }
    [Parameter] public bool IsEditModeEnabled { get; set; }
    [Parameter] public ParentEltContext ParentContext { get; set; }

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
        if (Value.Url == null) return;
        var json = new JsonObject();
        Value.ExtractJson(json);
        var dic = ConvertToDic(json);
        if(Value.IsAntiforgeryEnabled && Context.AntiforgeryToken != null)
            dic.Add(Context.AntiforgeryToken.FormField, Context.AntiforgeryToken.FormValue);

        var targetUrl = targetUrlHelperFactory.Build(Value.Url);
        var cookieContainer = new CookieContainer();
        using(var handler = new HttpClientHandler { CookieContainer = cookieContainer })
        {
            using (var httpClient = new HttpClient(handler))
            {
                var baseUrl = uriProvider.GetAbsoluteUriWithVirtualPath();
                var url = new Uri($"{baseUrl}{targetUrl}");
                var requestMessage = new HttpRequestMessage
                {
                    Content = new FormUrlEncodedContent(dic),
                    Method = HttpMethod.Post,
                    RequestUri = url
                };
                if (Value.IsAntiforgeryEnabled && Context.AntiforgeryToken != null)
                    cookieContainer.Add(new Uri(baseUrl), new Cookie(Context.AntiforgeryToken.CookieName, Context.AntiforgeryToken.CookieValue));

                var httpResult = await httpClient.SendAsync(requestMessage);
                string sss = "";
            }
        }
    }

    private RenderFragment CreateComponent() => builder =>
    {
        renderFormsElementsHelper.Render(builder, Value.Elements, Context, IsEditModeEnabled);
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
