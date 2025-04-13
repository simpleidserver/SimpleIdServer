using FormBuilder.Models;

namespace FormBuilder.Stores;

public interface ITemplateStore
{
    Task<Template> Get(string id, CancellationToken cancellationToken);
    Task<Template> GetActive(string realm, CancellationToken cancellationToken);
    Task<List<Template>> GetByName(string name, CancellationToken cancellationToken);
    void Add(Template record);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}
