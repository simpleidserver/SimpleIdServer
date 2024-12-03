namespace FormBuilder.Models;

public class WorkflowLinkSource
{
    public string EltId { get; set; }
    public Size Size { get; set; }
    public Coordinate CoordinateRelativeToStep { get; set; }

    public WorkflowLinkSource Clone()
    {
        return new WorkflowLinkSource
        {
            EltId = EltId,
            Size = Size.Clone(),
            CoordinateRelativeToStep = CoordinateRelativeToStep.Clone()
        };
    }
}
