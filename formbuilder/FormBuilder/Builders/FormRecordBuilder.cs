using FormBuilder.Models;

namespace FormBuilder.Builders;

public class FormRecordBuilder : BaseFormRecordCollectionBuilder<FormRecordBuilder, FormRecord>
{
    private FormRecordBuilder(string name, string correlationId, string realm, bool actAsStep) : base(new FormRecord
    {
        Id = Guid.NewGuid().ToString(),
        Name = name,
        CorrelationId = correlationId,
        Realm = realm,
        VersionNumber = 0,
        Status = RecordVersionStatus.Published,
        ActAsStep = actAsStep
    })
    {
    }

    public static FormRecordBuilder New(string name, string correlationId, string realm, bool actAsStep)
    {
        return new FormRecordBuilder(name, correlationId, realm, actAsStep);
    }
}
