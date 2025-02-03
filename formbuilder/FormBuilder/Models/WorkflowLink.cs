using System.Text.Json.Serialization;

namespace FormBuilder.Models;

public class WorkflowLink : ICloneable
{
    public string Id { get; set; }
    public WorkflowLinkSource Source { get; set; }
    public string SourceStepId { get; set; }
    public string TargetStepId { get; set; }
    public string ActionType { get; set; }
    public string? ActionParameter { get; set; }
    public string Description { get; set; }
    [JsonIgnore]
    public bool IsLinkHoverStep { get; set; }
    [JsonIgnore]
    public bool IsHover { get; set; }

    public object Clone()
    {
        return new WorkflowLink
        {
            Id = Id,
            Source = Source.Clone(),
            TargetStepId = TargetStepId,
            SourceStepId = SourceStepId,
            ActionType = ActionType,
            ActionParameter = ActionParameter
        };
    }

    public bool IsLinked(string stepId)
        => SourceStepId == stepId || TargetStepId == stepId;

    public static WorkflowLink Create(string sourceStepid, IFormElementRecord eltRecord, Coordinate coordinate, Size size, Coordinate coordinateRelativeToStep)
    {
        return new WorkflowLink
        {
            Id = Guid.NewGuid().ToString(),
            Source = new WorkflowLinkSource
            {
                EltId = eltRecord.Id,
                Size = size,
                CoordinateRelativeToStep = coordinateRelativeToStep
            },
            SourceStepId = sourceStepid
        };
    }
}
