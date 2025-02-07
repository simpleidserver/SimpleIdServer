using FormBuilder.Models;
using FormBuilder.Stores;
using Microsoft.EntityFrameworkCore;

namespace FormBuilder.EF.Stores;

public class FormStore : IFormStore
{
    private readonly FormBuilderDbContext _dbContext;

    public FormStore(FormBuilderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(FormRecord record)
        => _dbContext.Forms.Add(record);

    public Task<FormRecord> Get(string id, CancellationToken cancellationToken)
        => _dbContext.Forms.Include(f => f.AvailableStyles).SingleOrDefaultAsync(f => f.Id == id, cancellationToken);

    public Task<FormRecord> Get(string realm, string id, CancellationToken cancellationToken)
        => _dbContext.Forms.Include(f => f.AvailableStyles).SingleOrDefaultAsync(f => f.Id == id && f.Realm == realm, cancellationToken);

    public Task<List<FormRecord>> GetAll(CancellationToken cancellationToken)
        => _dbContext.Forms.ToListAsync(cancellationToken);

    public Task<List<FormRecord>> GetByCategory(string realm, string category, CancellationToken cancellationToken)
        => _dbContext.Forms.Where(f => f.Category == category && f.Realm == realm).ToListAsync(cancellationToken);

    public Task<List<FormRecord>> GetLatestPublishedVersionByCorrelationids(List<string> correlationIds, CancellationToken cancellationToken)
        => _dbContext.Forms.OrderByDescending(f => f.VersionNumber).Where(f => correlationIds.Contains(f.CorrelationId) && f.Status == RecordVersionStatus.Published).GroupBy(f => f.CorrelationId).Select(s => s.First()).ToListAsync(cancellationToken);

    public Task<FormRecord> GetLatestPublishedVersionByCorrelationId(string correlationId, CancellationToken cancellationToken)
    {
        return _dbContext.Forms.Where(f => f.Status == RecordVersionStatus.Published).OrderByDescending(f => f.VersionNumber).FirstOrDefaultAsync(f => f.CorrelationId == correlationId, cancellationToken);
    }

    public Task<FormRecord> GetLatestPublishedVersionByCorrelationId(string realm, string correlationId, CancellationToken cancellationToken)
    {
        return _dbContext.Forms.Where(f => f.Status == RecordVersionStatus.Published).OrderByDescending(f => f.VersionNumber).FirstOrDefaultAsync(f => f.CorrelationId == correlationId && f.Realm == realm, cancellationToken);
    }
    public Task<FormRecord> GetLatestVersionByCorrelationId(string realm, string correlationId, CancellationToken cancellationToken)
    {
        return _dbContext.Forms.OrderByDescending(f => f.VersionNumber).FirstOrDefaultAsync(f => f.CorrelationId == correlationId && f.Realm == realm, cancellationToken);
    }

    public Task<int> SaveChanges(CancellationToken cancellationToken)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
