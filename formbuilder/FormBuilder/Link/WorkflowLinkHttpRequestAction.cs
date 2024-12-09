using FormBuilder.Components;
using FormBuilder.Extensions;
using FormBuilder.Models;
using FormBuilder.Services;
using Microsoft.AspNetCore.Components.Rendering;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FormBuilder.Link;

public class WorkflowLinkHttpRequestAction : IWorkflowLinkAction
{
    private readonly IFormBuilderJsService _formBuilderJsService;

    public WorkflowLinkHttpRequestAction(IFormBuilderJsService formBuilderJsService)
    {
        _formBuilderJsService = formBuilderJsService; 
    }

    public string Type => ActionType;

    public static string ActionType => "HttpRequest";

    public string DisplayName => "Http request";

    public async Task Execute(WorkflowLink activeLink, WorkflowExecutionContext context)
    {
        if (string.IsNullOrWhiteSpace(activeLink.ActionParameter)) return;
        var parameter = JsonSerializer.Deserialize<WorkflowLinkHttpRequestParameter>(activeLink.ActionParameter);
        var cookieContainer = new CookieContainer();
        using (var handler = new HttpClientHandler { CookieContainer = cookieContainer, AllowAutoRedirect = false })
        {
            using (var httpClient = new HttpClient())
            {
                var dic = ConvertToDic(context.StepOutput);
                var target = new Uri(parameter.Target);
                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = target,
                    Content = new FormUrlEncodedContent(dic)
                };
                if (parameter.IsAntiforgeryEnabled && context.AntiforgeryToken != null)
                    cookieContainer.Add(target.GetBaseUri(), new Cookie(context.AntiforgeryToken.CookieName, context.AntiforgeryToken.CookieValue));

                var httpResult = await httpClient.SendAsync(requestMessage);
                var responseUri = httpResult.Headers.Location.AbsoluteUri;
                await _formBuilderJsService.Navigate(responseUri);
            }
        }
    }

    public void Render(RenderTreeBuilder builder, WorkflowLink workflowLink)
    {
        var parameter = new WorkflowLinkHttpRequestParameter();
        if (!string.IsNullOrWhiteSpace(workflowLink.ActionParameter))
            parameter = JsonSerializer.Deserialize<WorkflowLinkHttpRequestParameter>(workflowLink.ActionParameter);
        builder.OpenComponent<WorkflowLinkHttpRequestComponent>(0);
        builder.AddAttribute(1, nameof(WorkflowLinkHttpRequestComponent.Parameter), parameter);
        builder.AddAttribute(2, nameof(WorkflowLinkHttpRequestComponent.WorkflowLink), workflowLink);
        builder.CloseComponent();
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
