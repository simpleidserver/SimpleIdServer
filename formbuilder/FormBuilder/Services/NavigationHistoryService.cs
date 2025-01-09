using FormBuilder.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Reflection.Metadata;

namespace FormBuilder.Services;

public interface INavigationHistoryService
{
    Task SaveCurrentStep(WorkflowContext context);
    Task SaveExecutedLink(WorkflowContext context, string linkId);
    Task Back(WorkflowContext context);
}

public class NavigationHistoryService : INavigationHistoryService
{
    private readonly NavigationManager _navigationManager;
    private readonly ProtectedSessionStorage _sessionStorage;
    private readonly IFormBuilderJsService _formBuilderJsService;

    public NavigationHistoryService(NavigationManager navigationManager, ProtectedSessionStorage sessionStorage, IFormBuilderJsService formBuilderJsService)
    {
        _navigationManager = navigationManager;
        _sessionStorage = sessionStorage;
        _formBuilderJsService = formBuilderJsService;
    }

    public async Task SaveCurrentStep(WorkflowContext context)
    {
        var uri = _navigationManager.Uri;
        await _sessionStorage.SetAsync($"Step.{context.Definition.Workflow.Id}.{context.Execution.CurrentStepId}", uri);
    }

    public async Task SaveExecutedLink(WorkflowContext context, string linkId)
    {
        var name = $"Step.{context.Definition.Workflow.Id}.executedLinks";
        var links = new List<string>();
        try
        {
            var res = await _sessionStorage.GetAsync<List<string>>(name);
            links = res.Success ? res.Value : new List<string>();
        }
        catch (Exception ex) { }
        if(!links.Contains(linkId)) links.Add(linkId);
        await _sessionStorage.SetAsync(name, links);
    }

    public async Task Back(WorkflowContext context)
    {
        List<string> executedLinks = new List<string>();
        try
        {
            var res = await _sessionStorage.GetAsync<List<string>>($"Step.{context.Definition.Workflow.Id}.executedLinks");
            if (!res.Success) return;
            executedLinks = res.Value;
        }
        catch
        {
            return;
        }

        var allLinks = context.Definition.Workflow.Links;
        for(var i = executedLinks.Count - 1; i >= 0; i--)
        {
            var linkId = executedLinks[i];
            var link = allLinks.FirstOrDefault(l => l.Id == linkId);
            if (link == null) continue;
            var linkDef = context.Definition.Workflow.Links.SingleOrDefault(l => l.Id == link.Id);
            if (linkDef == null || context.Execution.CurrentStepId == linkDef.SourceStepId) continue;
            var uri = await _sessionStorage.GetAsync<string>($"Step.{context.Definition.Workflow.Id}.{linkDef.SourceStepId}");
            if (!uri.Success) return;
            await _formBuilderJsService.NavigateForce(uri.Value);
        }
    }
}
