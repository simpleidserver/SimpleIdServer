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

    public Task<WorkflowRecord> Get(string id, CancellationToken cancellationToken)
        => _dbContext.Workflows.SingleOrDefaultAsync(w => w.Id == id, cancellationToken);
}
