using FormBuilder.Models;
using FormBuilder.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FormBuilder.EF.Stores;

public class WorkflowStore : IWorkflowStore
{
    private readonly FormBuilderDbContext _dbContext;

    public WorkflowStore(FormBuilderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<WorkflowRecord> Get(string realm, string id, CancellationToken cancellationToken)
        => _dbContext.Workflows
            .Include(w => w.Links)
            .Include(w => w.Steps)
            .SingleOrDefaultAsync(w => w.Id == id && w.Realm == realm, cancellationToken);

    public Task<List<WorkflowRecord>> GetAll(string realm, CancellationToken cancellationToken)
        => _dbContext.Workflows
            .Include(w => w.Links)
            .Include(w => w.Steps)
            .Where(w => w.Realm == realm).ToListAsync(cancellationToken);

    public void Add(WorkflowRecord record)
        => _dbContext.Workflows.Add(record);

    public void Delete(WorkflowRecord record)
        => _dbContext.Workflows.Remove(record);

    public Task<int> SaveChanges(CancellationToken cancellationToken)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
