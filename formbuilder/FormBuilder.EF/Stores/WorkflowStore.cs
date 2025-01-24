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

    public void Add(WorkflowRecord record)
        => _dbContext.Workflows.Add(record);

    public Task<WorkflowRecord> Get(string id, CancellationToken cancellationToken)
        => _dbContext.Workflows
            .Include(w => w.Links)
            .Include(w => w.Steps)
            .SingleOrDefaultAsync(w => w.Id == id, cancellationToken);

    public Task<WorkflowRecord> GetLatestPublishedRecord(string correlationId, CancellationToken cancellationToken)
    {
        return _dbContext.Workflows.Where(w => w.Status == RecordVersionStatus.Published).OrderByDescending(w => w.VersionNumber).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<int> SaveChanges(CancellationToken cancellationToken)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
