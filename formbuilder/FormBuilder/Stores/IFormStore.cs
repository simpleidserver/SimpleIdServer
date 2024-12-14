using FormBuilder.Models;

namespace FormBuilder.Stores;

public interface IFormStore
{
    Task<FormRecord> Get(string name, CancellationToken cancellationToken);
    Task<List<FormRecord>> GetAll(CancellationToken cancellationToken);
}
