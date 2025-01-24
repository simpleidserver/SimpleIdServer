using FormBuilder.Models;

namespace FormBuilder.Stores;

public interface IFormStore
{
    Task<FormRecord> Get(string id, CancellationToken cancellationToken);
    Task<List<FormRecord>> GetAll(CancellationToken cancellationToken);
    Task<FormRecord> GetLatestPublishedVersionByCorrelationId(string correlationId, CancellationToken cancellationToken);
    void Add(FormRecord record);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}