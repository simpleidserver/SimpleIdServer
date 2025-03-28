using FormBuilder.Models;

namespace FormBuilder.Repositories;

public interface IWorkflowStore
{
    Task<WorkflowRecord> Get(string realm, string id, CancellationToken cancellationToken);
    Task<WorkflowRecord> GetByName(string realm, string name, CancellationToken cancellationToken);
    Task<List<WorkflowRecord>> GetAll(string realm, CancellationToken cancellationToken);
    void Add(WorkflowRecord record);
    void Delete(WorkflowRecord record);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}
