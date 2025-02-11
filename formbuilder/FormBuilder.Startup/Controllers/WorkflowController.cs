using FormBuilder.Repositories;
using FormBuilder.Startup.Controllers.ViewModels;
using FormBuilder.Stores;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Startup.Controllers;

public class WorkflowController : Controller
{
    private readonly IEnumerable<IWorkflowLayoutService> _workflowLayoutServices;
    private readonly IWorkflowStore _workflowStore;
    private readonly IFormStore _formStore;

    public WorkflowController(IEnumerable<IWorkflowLayoutService> workflowLayoutServices, IWorkflowStore workflowStore, IFormStore formStore)
    {
        _workflowLayoutServices = workflowLayoutServices;
        _workflowStore = workflowStore;
        _formStore = formStore;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var workflowLayouts = _workflowLayoutServices.Select(s => s.Get()).ToList();
        var formCorrelationIds = workflowLayouts.SelectMany(s => s.Links).Select(s => s.TargetFormCorrelationId).Distinct().ToList()
            .Concat(workflowLayouts.Select(s => s.SourceFormCorrelationId).Distinct().ToList())
            .ToList();
        var forms = await _formStore.GetLatestPublishedVersionByCorrelationids(formCorrelationIds, cancellationToken);
        var workflow = await _workflowStore.GetLatest(Constants.DefaultRealm, "pwdMobile", cancellationToken);
        var viewModel = new WorkflowIndexViewModel
        {
            Forms = forms,
            WorkflowLayouts = workflowLayouts,
            Workflow = workflow
        };
        return View(viewModel);
    }
}