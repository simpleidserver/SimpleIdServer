using FormBuilder.Helpers;
using FormBuilder.Models;
using FormBuilder.Stores;

namespace FormBuilder.Services;

public interface IVersionedFormService
{
    Task<FormRecord> Publish(FormRecord record, CancellationToken cancellationToken);
}

public class VersionedFormService : BaseGenericVersionedRecordService<FormRecord>, IVersionedFormService
{
    private readonly IFormStore _formStore;

    public VersionedFormService(IDateTimeHelper dateTimeHelper, IFormStore formStore) : base(dateTimeHelper)
    {
        _formStore = formStore;
    }

    protected override async Task Add(FormRecord record, CancellationToken cancellationToken)
    {
        _formStore.Add(record);
        await _formStore.SaveChanges(cancellationToken);
    }

    public override Task<FormRecord> GetLatestPublishedRecord(string correlationId, CancellationToken cancellationToken)
        => _formStore.GetLatestPublishedVersionByCorrelationId(correlationId, cancellationToken);
}
