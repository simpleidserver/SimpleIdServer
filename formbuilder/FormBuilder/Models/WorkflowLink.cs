using Radzen.Blazor.Rendering;

namespace FormBuilder.Models;

public class WorkflowLink
{
    public Coordinate SourceCoordinate { get; set; }
    public Coordinate TargetCoordinate { get; set; }
    public AnchorDirections SourceDirection { get; set; }
    public AnchorDirections TargetDirection { get; set; }
    public string SourceStepId { get; set; }
    public string TargetStepId { get; set; }

    public void UpdateSourceCoordinate(WorkflowStep step)
    {
        var offsetWidth = step.Size.width;
        var offsetHeight = step.Size.height;
        switch (SourceDirection)
        {
            case AnchorDirections.TOP:
                offsetHeight = 0;
                offsetWidth = offsetWidth / 2;
                break;
            case AnchorDirections.BOTTOM:
                offsetWidth = offsetWidth / 2;
                break;
            case AnchorDirections.LEFT:
                offsetHeight = step.Size.height / 2;
                offsetWidth = 0;
                break;
            case AnchorDirections.RIGHT:
                offsetHeight = step.Size.height / 2;
                break;
        }
        SourceCoordinate = new Coordinate
        {
            X = offsetWidth + Step.Coordinate.X,
            Y = offsetHeight + Step.Coordinate.Y
        };
        TargetCoordinate = new Coordinate
        {
            X = offsetWidth + Step.Coordinate.X,
            Y = offsetHeight + Step.Coordinate.Y
        };
    }
}
