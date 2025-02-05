using FormBuilder.Helpers;
using FormBuilder.Models;
using FormBuilder.Repositories;

namespace FormBuilder.Services;

public interface IVersionedWorkflowService
{
    Task<WorkflowRecord> GetLatestPublishedRecord(string correlationId, CancellationToken cancellationToken);
    Task<WorkflowRecord> Publish(WorkflowRecord record, CancellationToken cancellationToken);
}

public class VersionedWorkflowService : BaseGenericVersionedRecordService<WorkflowRecord>, IVersionedWorkflowService
{
    private readonly IWorkflowStore _workflowStore;

    public VersionedWorkflowService(IWorkflowStore workflowStore, IDateTimeHelper dateTimeHelper) : base(dateTimeHelper)
    {
        _workflowStore = workflowStore;
    }

    public override Task<WorkflowRecord> GetLatestPublishedRecord(string correlationId, CancellationToken cancellationToken)
        => _workflowStore.GetLatestPublishedRecord(correlationId, cancellationToken);

    protected override async Task Add(WorkflowRecord record, CancellationToken cancellationToken)
    {
        _workflowStore.Add(record);
        await _workflowStore.SaveChanges(cancellationToken);
    }
}
