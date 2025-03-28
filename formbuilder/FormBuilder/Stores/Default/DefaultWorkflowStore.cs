using FormBuilder.Models;
using FormBuilder.Repositories;

namespace FormBuilder.Stores.Default;

public class DefaultWorkflowStore : IWorkflowStore
{
    private readonly List<WorkflowRecord> _workflows;

    public DefaultWorkflowStore(List<WorkflowRecord> workflows)
    {
        _workflows = workflows;
    }

    public Task<WorkflowRecord> Get(string realm, string id, CancellationToken cancellationToken)
    {
        var result = _workflows.SingleOrDefault(w => w.Id == id && w.Realm == realm);
        return Task.FromResult(result);
    }

    public Task<WorkflowRecord> GetByName(string realm, string name, CancellationToken cancellationToken)
    {
        var result = _workflows.SingleOrDefault(w => w.Name == name && w.Realm == realm);
        return Task.FromResult(result);
    }

    public Task<List<WorkflowRecord>> GetAll(string realm, CancellationToken cancellationToken)
    {
        var result = _workflows.Where(w => w.Realm == realm).ToList();
        return Task.FromResult(result);
    }

    public void Add(WorkflowRecord record)
        => _workflows.Add(record);

    public void Delete(WorkflowRecord record)
        => _workflows.Remove(record);

    public Task<int> SaveChanges(CancellationToken cancellationToken)
        => Task.FromResult(0);
}
