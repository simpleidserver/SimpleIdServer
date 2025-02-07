using FormBuilder.Models;

namespace FormBuilder.Stores;

public interface IFormStore
{
    Task<FormRecord> Get(string id, CancellationToken cancellationToken);
    Task<FormRecord> Get(string realm, string id, CancellationToken cancellationToken);
    Task<List<FormRecord>> GetAll(CancellationToken cancellationToken);
    Task<FormRecord> GetLatestPublishedVersionByCorrelationId(string correlationId, CancellationToken cancellationToken);
    Task<FormRecord> GetLatestPublishedVersionByCorrelationId(string realm, string correlationId, CancellationToken cancellationToken);
    Task<FormRecord> GetLatestVersionByCorrelationId(string realm, string correlationId, CancellationToken cancellationToken);
    Task<List<FormRecord>> GetByCategory(string realm, string category, CancellationToken cancellationToken);
    Task<List<FormRecord>> GetLatestPublishedVersionByCorrelationids(List<string> correlationIds, CancellationToken cancellationToken);
    void Add(FormRecord record);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}