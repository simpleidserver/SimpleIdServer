namespace FormBuilder.Models;

public class WorkflowStep : ICloneable
{
    public string Id { get; set; }
    public string FormRecordCorrelationId { get; set; }
    public bool IsEmptyStep
    {
        get
        {
            return FormRecordCorrelationId == Constants.EmptyStep.CorrelationId;
        }
    }

    public object Clone()
    {
        return new WorkflowStep
        {
            Id = Id,
            FormRecordCorrelationId = FormRecordCorrelationId
        };
    }
}