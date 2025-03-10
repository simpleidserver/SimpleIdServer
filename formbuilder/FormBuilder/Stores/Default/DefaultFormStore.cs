using FormBuilder.Models;
using System.Linq.Dynamic.Core;

namespace FormBuilder.Stores.Default;

public class DefaultFormStore : IFormStore
{
    private readonly List<FormRecord> _forms;

    public DefaultFormStore(List<FormRecord> forms)
    {
        _forms = forms;
    }

    public void Add(FormRecord record) => _forms.Add(record);

    public Task<FormRecord> Get(string id, CancellationToken cancellationToken)
    {
        var result = _forms.SingleOrDefault(f => f.Id == id);
        return Task.FromResult(result);
    }

    public Task<FormRecord> Get(string realm, string id, CancellationToken cancellationToken)
    {
        var result = _forms.SingleOrDefault(f => f.Id == id && f.Realm == realm);
        return Task.FromResult(result);
    }

    public Task<List<FormRecord>> GetAll(string realm, CancellationToken cancellationToken)
    {
        var result = _forms.Where(f => f.Realm == realm).ToList();
        return Task.FromResult(result);
    }

    public Task<List<FormRecord>> GetAll(CancellationToken cancellationToken)
    {
        return Task.FromResult(_forms.ToList());
    }

    public Task<List<FormRecord>> GetLatestPublishedVersionByCategory(string realm, string category, CancellationToken cancellationToken)
    {
        var result = _forms
            .Where(f => f.Category == category && f.Realm == realm && f.Status == RecordVersionStatus.Published)
            .OrderByDescending(f => f.VersionNumber)
            .GroupBy(f => f.CorrelationId)
            .Select(g => g.First())
            .ToList();
        return Task.FromResult(result);
    }

    public Task<List<FormRecord>> GetByCategory(string realm, string category, CancellationToken cancellationToken)
    {
        var result = _forms
            .Where(f => f.Category == category && f.Realm == realm)
            .OrderByDescending(f => f.VersionNumber)
            .GroupBy(f => f.CorrelationId)
            .Select(g => g.First())
            .ToList();
        return Task.FromResult(result);
    }

    public Task<List<FormRecord>> GetLatestPublishedVersionByCorrelationids(List<string> correlationIds, CancellationToken cancellationToken)
    {
        var result = _forms
            .Where(f => correlationIds.Contains(f.CorrelationId) && f.Status == RecordVersionStatus.Published)
            .OrderByDescending(f => f.VersionNumber)
            .GroupBy(f => f.CorrelationId)
            .Select(g => g.First())
            .ToList();
        return Task.FromResult(result);
    }

    public Task<FormRecord> GetLatestPublishedVersionByCorrelationId(string correlationId, CancellationToken cancellationToken)
    {
        var result = _forms
            .Where(f => f.Status == RecordVersionStatus.Published && f.CorrelationId == correlationId)
            .OrderByDescending(f => f.VersionNumber)
            .FirstOrDefault();
        return Task.FromResult(result);
    }

    public Task<FormRecord> GetLatestPublishedVersionByCorrelationId(string realm, string correlationId, CancellationToken cancellationToken)
    {
        var result = _forms
            .Where(f => f.Status == RecordVersionStatus.Published && f.CorrelationId == correlationId && f.Realm == realm)
            .OrderByDescending(f => f.VersionNumber)
            .FirstOrDefault();
        return Task.FromResult(result);
    }

    public Task<FormRecord> GetLatestVersionByCorrelationId(string realm, string correlationId, CancellationToken cancellationToken)
    {
        var result = _forms
            .Where(f => f.CorrelationId == correlationId && f.Realm == realm)
            .OrderByDescending(f => f.VersionNumber)
            .FirstOrDefault();
        return Task.FromResult(result);
    }

    public Task<int> SaveChanges(CancellationToken cancellationToken)
    {
        return Task.FromResult(0);
    }
}
