using FormBuilder.Models;

namespace FormBuilder.Stores;

public interface IFormStore
{
    Task<FormRecord> Get(string id, CancellationToken cancellationToken);
    Task<List<FormRecord>> GetAll(CancellationToken cancellationToken);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}
