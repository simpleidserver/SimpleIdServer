using FormBuilder.Components;
using FormBuilder.Extensions;
using FormBuilder.Link.Components;
using FormBuilder.Models;
using FormBuilder.Services;
using FormBuilder.UIs;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FormBuilder.Link;

public class WorkflowLinkHttpRequestAction : IWorkflowLinkAction
{
    private readonly IFormBuilderJsService _formBuilderJsService;
    private readonly FormBuilderOptions _options;

    public WorkflowLinkHttpRequestAction(IFormBuilderJsService formBuilderJsService, IOptions<FormBuilderOptions> options)
    {
        _formBuilderJsService = formBuilderJsService;
        _options = options.Value;
    }

    public string Type => ActionType;

    public static string ActionType => "HttpRequest";

    public string DisplayName => "Http request";

    public List<string> ExcludedStepNames => new List<string>();

    public bool CanBeAppliedMultipleTimes => false;

    public async Task Execute(WorkflowLink activeLink, WorkflowExecutionContext context)
    {
        if (string.IsNullOrWhiteSpace(activeLink.ActionParameter)) return;
        var parameter = JsonSerializer.Deserialize<WorkflowLinkHttpRequestParameter>(activeLink.ActionParameter);
        var cookieContainer = new CookieContainer();
        using (var handler = new HttpClientHandler { CookieContainer = cookieContainer, AllowAutoRedirect = false })
        {
            using (var httpClient = new HttpClient(handler))
            {
                var json = context.StepOutput;
                if (parameter.IsAntiforgeryEnabled && context.AntiforgeryToken != null)
                    json.Add(context.AntiforgeryToken.FormField, context.AntiforgeryToken.FormValue);

                var dic = ConvertToDic(json);
                var target = new Uri(parameter.Target);
                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = target,
                    Content = new FormUrlEncodedContent(dic)
                };

                requestMessage.Headers.Add(_options.CurrentWorkflowHeaderName, 
                    JsonSerializer.Serialize(new ExecutedLink { CurrentStepId = context.CurrentStepId, WorkflowId = context.Workflow.Id, CurrentLink = activeLink.Id })
                );
                if (parameter.IsAntiforgeryEnabled && context.AntiforgeryToken != null)
                    cookieContainer.Add(target.GetBaseUri(), new Cookie(context.AntiforgeryToken.CookieName, context.AntiforgeryToken.CookieValue));

                var httpResult = await httpClient.SendAsync(requestMessage);
                var baseUrl = target.GetBaseUri().ToString().TrimEnd('/');
                var responseUri = $"{baseUrl}{httpResult.Headers.Location.OriginalString}";
                await _formBuilderJsService.NavigateForce(responseUri);
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
