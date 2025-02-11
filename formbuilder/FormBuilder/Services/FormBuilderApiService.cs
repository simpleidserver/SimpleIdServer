using FormBuilder.Controllers;
using FormBuilder.Helpers;
using FormBuilder.Models;
using System.Text;
using System.Text.Json;

namespace FormBuilder.Services;

public interface IFormBuilderApiService
{
    Task UpdateCss(string formId, string css, CancellationToken cancellationToken);
    Task UpdateForm(FormRecord formRecord, CancellationToken cancellationToken);
    Task<FormRecord> PublishForm(string id, CancellationToken cancellationToken);
    Task UpdateWorkflow(WorkflowRecord workflow, CancellationToken cancellationToken);
    Task<WorkflowRecord> PublishWorkflow(string id, CancellationToken cancellationToken);
}

public class FormBuilderApiService : IFormBuilderApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IUriProvider _uriProvider;

    public FormBuilderApiService(IHttpClientFactory httpClientFactory, IUriProvider uriProvider)
    {
        _httpClientFactory = httpClientFactory;
        _uriProvider = uriProvider;
    }

    public async Task UpdateCss(string formId, string css, CancellationToken cancellationToken)
    {
        using (var httpClient = _httpClientFactory.CreateClient())
        {
            var url = _uriProvider.GetActiveFormCssUrl(formId);
            var requestMessage = new HttpRequestMessage
            {
                Content = new StringContent(JsonSerializer.Serialize(new UpdateFormStyleCommand { Content = css }), Encoding.UTF8, "application/json"),
                Method = HttpMethod.Put,
                RequestUri = new Uri(url)
            };
            await httpClient.SendAsync(requestMessage);
        }
    }

    public async Task UpdateForm(FormRecord form, CancellationToken cancellationToken)
    {
        using (var httpClient = _httpClientFactory.CreateClient())
        {
            var url = _uriProvider.GetFormUrl(form.Id);
            var requestMessage = new HttpRequestMessage
            {
                Content = new StringContent(JsonSerializer.Serialize(new UpdateFormCommand { Form = form }), Encoding.UTF8, "application/json"),
                Method = HttpMethod.Put,
                RequestUri = new Uri(url)
            };
            await httpClient.SendAsync(requestMessage);
        }
    }

    public async Task<FormRecord> PublishForm(string id, CancellationToken cancellationToken)
    {
        using (var httpClient = _httpClientFactory.CreateClient())
        {
            var url = _uriProvider.GetFormPublishUrl(id);
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url)
            };
            var httpResult = await httpClient.SendAsync(requestMessage, cancellationToken);
            var json = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<FormRecord>(json);
        }
    }

    public async Task UpdateWorkflow(WorkflowRecord workflow, CancellationToken cancellationToken)
    {
        using (var httpClient = _httpClientFactory.CreateClient())
        {
            var url = _uriProvider.GetWorkflowUrl(workflow.Id);
            var requestMessage = new HttpRequestMessage
            {
                Content = new StringContent(JsonSerializer.Serialize(new UpdateWorkflowCommand { Workflow = workflow }), Encoding.UTF8, "application/json"),
                Method = HttpMethod.Put,
                RequestUri = new Uri(url)
            };
            await httpClient.SendAsync(requestMessage);
        }
    }

    public async Task<WorkflowRecord> PublishWorkflow(string id, CancellationToken cancellationToken)
    {
        using (var httpClient = _httpClientFactory.CreateClient())
        {
            var url = _uriProvider.GetWorkflowPublishUrl(id);
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url)
            };
            var httpResult = await httpClient.SendAsync(requestMessage, cancellationToken);
            var json = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<WorkflowRecord>(json);
        }
    }
}
