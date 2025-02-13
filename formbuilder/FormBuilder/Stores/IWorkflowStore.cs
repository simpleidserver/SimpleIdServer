using FormBuilder.Models;

namespace FormBuilder.Repositories;

public interface IWorkflowStore
{
    Task<WorkflowRecord> Get(string realm, string id, CancellationToken cancellationToken);
    void Add(WorkflowRecord record);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}
