using FormBuilder.Repositories;
using FormBuilder.Stores;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FormBuilder.UIs;

public class BaseWorkflowController : Controller
{
    private readonly IAntiforgery _antiforgery;
    private readonly IWorkflowStore _workflowStore;
    private readonly IFormStore _formStore;
    private readonly FormBuilderOptions _options;

    public BaseWorkflowController(IAntiforgery antiforgery, IWorkflowStore workflowStore, IFormStore formStore, IOptions<FormBuilderOptions> options)
    {
        _antiforgery = antiforgery;
        _workflowStore = workflowStore;
        _formStore = formStore;
        _options = options.Value;
    }

    protected async Task<WorkflowViewModel> Get(string workflowId, string stepId, CancellationToken cancellationToken)
    {
        var workflow = await _workflowStore.Get(workflowId, cancellationToken);
        var records = await _formStore.GetAll(cancellationToken);
        var step = workflow.GetStep(stepId);
        var tokenSet = _antiforgery.GetAndStoreTokens(HttpContext);
        return new WorkflowViewModel
        {
            CurrentStepId= step.Id,
            Workflow = workflow,
            AntiforgeryToken = new AntiforgeryTokenRecord
            {
                FormValue = tokenSet.RequestToken,
                FormField = tokenSet.FormFieldName,
                CookieName = _options.AntiforgeryCookieName,
                CookieValue = tokenSet.CookieToken
            },
            FormRecords = records
        };
    }
}
