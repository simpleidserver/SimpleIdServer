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

    public Task<WorkflowRecord> GetLatest(string realm, string correlationId, CancellationToken cancellationToken)
        => _dbContext.Workflows.Include(w => w.Links).Include(w => w.Steps).OrderByDescending(w => w.VersionNumber).FirstOrDefaultAsync(w => w.Realm == realm && w.CorrelationId == correlationId, cancellationToken);

    public Task<WorkflowRecord> GetLatestPublishedRecord(string correlationId, CancellationToken cancellationToken)
    {
        return _dbContext.Workflows.Include(w => w.Links).Include(w => w.Steps).Where(w => w.Status == RecordVersionStatus.Published && w.CorrelationId == correlationId).OrderByDescending(w => w.VersionNumber).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<WorkflowRecord> GetLatestPublishedRecord(string realm, string correlationId, CancellationToken cancellationToken)
    {
        return _dbContext.Workflows.Include(w => w.Links).Include(w => w.Steps).Where(w => w.Status == RecordVersionStatus.Published && w.CorrelationId == correlationId && w.Realm == realm).OrderByDescending(w => w.VersionNumber).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<int> SaveChanges(CancellationToken cancellationToken)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
