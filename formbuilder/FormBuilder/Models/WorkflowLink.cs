using FormBuilder.Link;
using System.Text.Json.Serialization;

namespace FormBuilder.Models;

public class WorkflowLink
{
    public string Id { get; set; }
    public Coordinate SourceCoordinate { get; set; }
    public Coordinate TargetCoordinate { get; set; }
    public WorkflowLinkSource Source { get; set; }
    public string SourceStepId { get; set; }
    public string TargetStepId { get; set; }
    public string ActionType { get; set; }
    public string? ActionParameter { get; set; }
    [JsonIgnore]
    public bool IsLinkHoverStep { get; set; }
    [JsonIgnore]
    public bool IsHover { get; set; }

    public WorkflowLink Clone()
    {
        return new WorkflowLink
        {
            SourceCoordinate = SourceCoordinate.Clone(),
            TargetCoordinate = TargetCoordinate.Clone(),
            Source = Source.Clone(),
            TargetStepId = TargetStepId,
            SourceStepId = SourceStepId,
            ActionType = ActionType,
            ActionParameter = ActionParameter
        };
    }

    public bool IsLinked(string stepId)
        => SourceStepId == stepId || TargetStepId == stepId;

    public Coordinate ComputeTextInfoCoordinate()
    {
        var result = (TargetCoordinate - SourceCoordinate).Positive().Div(2).Round();
        var minCoordinate = SourceCoordinate.Min(TargetCoordinate);
        return result + minCoordinate;
    }

    public static WorkflowLink Create(string sourceStepid, IFormElementRecord eltRecord, Coordinate coordinate, Size size, Coordinate coordinateRelativeToStep)
    {
        var anchorCoordinate = GetAnchorCoordinate(size, coordinate, AnchorDirections.RIGHT);
        return new WorkflowLink
        {
            Source = new WorkflowLinkSource
            {
                EltId = eltRecord.Id,
                Size = size,
                CoordinateRelativeToStep = coordinateRelativeToStep
            },
            SourceStepId = sourceStepid,
            SourceCoordinate = anchorCoordinate,
            TargetCoordinate = anchorCoordinate.Clone()
        };
    }

    public void UpdateCoordinate(WorkflowStep step)
    {
        if (step.Id == TargetStepId)
        {
            var coordinate = GetAnchorCoordinate(step.Size, step.Coordinate, AnchorDirections.LEFT);
            TargetCoordinate = coordinate;
            return;
        }

        var res = step.Coordinate + Source.CoordinateRelativeToStep;
        SourceCoordinate = GetAnchorCoordinate(Source.Size, res, AnchorDirections.RIGHT);
    }

    private static Coordinate GetAnchorCoordinate(Size size, Coordinate coordinate, AnchorDirections direction)
    {
        var offsetWidth = size.width;
        var offsetHeight = size.height;
        switch (direction)
        {
            case AnchorDirections.TOP:
                offsetHeight = 0;
                offsetWidth = offsetWidth / 2;
                break;
            case AnchorDirections.BOTTOM:
                offsetWidth = offsetWidth / 2;
                break;
            case AnchorDirections.LEFT:
                offsetHeight = size.height / 2;
                offsetWidth = 0;
                break;
            case AnchorDirections.RIGHT:
                offsetHeight = size.height / 2;
                break;
        }

        var result = new Coordinate
        {
            X = offsetWidth + coordinate.X,
            Y = offsetHeight + coordinate.Y
        };
        return result;
    }
}
