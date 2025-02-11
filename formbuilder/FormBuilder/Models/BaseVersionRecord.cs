namespace FormBuilder.Models;

public abstract class BaseVersionRecord
{
    public string CorrelationId { get; set; }
    public RecordVersionStatus Status { get; set; }
    public int VersionNumber { get; set; }
    public DateTime UpdateDateTime { get; set; }

    public abstract BaseVersionRecord NewDraft(DateTime currentDateTime);

    public void Publish(DateTime currentDateTime)
    {
        Status = RecordVersionStatus.Published;
        UpdateDateTime = currentDateTime;
    }
}