using FormBuilder.Startup.Controllers.ViewModels;
using FormBuilder.Stores;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Startup.Controllers;

public class WorkflowController : Controller
{
    private readonly IEnumerable<IWorkflowLayoutService> _workflowLayoutServices;
    private readonly IFormStore _formStore;

    public WorkflowController(IEnumerable<IWorkflowLayoutService> workflowLayoutServices, IFormStore formStore)
    {
        _workflowLayoutServices = workflowLayoutServices;
        _formStore = formStore;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var workflowLayouts = _workflowLayoutServices.Select(s => s.Get()).ToList();
        var formCorrelationIds = workflowLayouts.SelectMany(s => s.Links).Select(s => s.TargetFormCorrelationId).Distinct().ToList()
            .Concat(workflowLayouts.Select(s => s.SourceFormCorrelationId).Distinct().ToList())
            .ToList();
        var forms = await _formStore.GetLatestPublishedVersionByCorrelationids(formCorrelationIds, cancellationToken);
        var viewModel = new WorkflowIndexViewModel
        {
            Forms = forms,
            WorkflowLayouts = workflowLayouts,
            Workflow = new Models.WorkflowRecord
            {
                Id = Guid.NewGuid().ToString(),
                CorrelationId = Guid.NewGuid().ToString()
            }
        };
        return View(viewModel);
    }
}