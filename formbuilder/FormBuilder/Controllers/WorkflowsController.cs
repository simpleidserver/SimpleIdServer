using FormBuilder.Helpers;
using FormBuilder.Repositories;
using FormBuilder.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace FormBuilder.Controllers;

[Route("workflows")]
public class WorkflowsController : Controller
{
    private readonly IWorkflowStore _workflowStore;
    private readonly IDateTimeHelper _dateTimeHelper;
    private readonly IVersionedWorkflowService _versionedWorkflowService;

    public WorkflowsController(IWorkflowStore workflowStore, IDateTimeHelper dateTimeHelper, IVersionedWorkflowService versionedWorkflowService)
    {
        _workflowStore = workflowStore;
        _dateTimeHelper = dateTimeHelper;
        _versionedWorkflowService = versionedWorkflowService;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateWorkflowCommand command, CancellationToken cancellationToken)
    {
        var workflow = await _workflowStore.Get(id, cancellationToken);
        if (workflow == null) return new NoContentResult();
        workflow.Update(command.Workflow.Steps, command.Workflow.Links, _dateTimeHelper.GetCurrent());
        await _workflowStore.SaveChanges(cancellationToken);
        return new NoContentResult();
    }

    [HttpPost("{id}/publish")]
    public async Task<IActionResult> Publish(string id, CancellationToken cancellationToken)
    {
        var workflow = await _workflowStore.Get(id, cancellationToken);
        if (workflow == null) return new NoContentResult();
        var newWorkflow = await _versionedWorkflowService.Publish(workflow, cancellationToken);
        return new ContentResult
        {
            StatusCode = (int)HttpStatusCode.Created,
            Content = JsonSerializer.Serialize(newWorkflow),
            ContentType = "application/json"
        };
    }
}
