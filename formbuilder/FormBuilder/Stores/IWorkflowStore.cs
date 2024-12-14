using FormBuilder.Models;

namespace FormBuilder.Repositories;

public interface IWorkflowStore
{
    Task<WorkflowRecord> Get(string id, CancellationToken cancellationToken);
}
