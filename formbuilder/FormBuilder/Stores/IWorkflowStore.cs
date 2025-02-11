using FormBuilder.Models;

namespace FormBuilder.Repositories;

public interface IWorkflowStore
{
    Task<WorkflowRecord> Get(string id, CancellationToken cancellationToken);
    Task<WorkflowRecord> GetLatest(string realm, string correlationId, CancellationToken cancellationToken);
    Task<WorkflowRecord> GetLatestPublishedRecord(string correlationId, CancellationToken cancellationToken);
    Task<WorkflowRecord> GetLatestPublishedRecord(string realm, string correlationId, CancellationToken cancellationToken);
    void Add(WorkflowRecord record);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}
