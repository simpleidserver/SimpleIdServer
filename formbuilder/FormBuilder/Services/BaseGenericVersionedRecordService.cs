using FormBuilder.Helpers;
using FormBuilder.Models;

namespace FormBuilder.Services;

public abstract class BaseGenericVersionedRecordService<T> where T : BaseVersionRecord
{
    private readonly IDateTimeHelper _dateTimeHelper;

    public BaseGenericVersionedRecordService(IDateTimeHelper dateTimeHelper)
    {
        _dateTimeHelper = dateTimeHelper;
    }

    public async Task<T> Publish(T record, CancellationToken cancellationToken)
    {
        var currentDateTime = _dateTimeHelper.GetCurrent();
        record.Publish(currentDateTime);
        var newDraft = record.NewDraft(currentDateTime) as T;
        await Add(newDraft, cancellationToken);
        return newDraft;
    }
    public abstract Task<T> GetLatestPublishedRecord(string correlationId, CancellationToken cancellationToken);

    protected abstract Task Add(T record, CancellationToken cancellationToken);
}
