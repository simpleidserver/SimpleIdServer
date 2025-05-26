using System.Text.Json.Serialization;

namespace FormBuilder.Models;

public class WorkflowLink : ICloneable
{
    public string Id { get; set; }
    public WorkflowLinkSource Source { get; set; }
    public string SourceStepId { get; set; }
    public string? TargetStepId { get; set; }
    public string? ActionType { get; set; }
    public string? ActionParameter { get; set; }
    public bool IsMainLink { get; set; }
    public List<WorkflowLinkTarget> Targets
    {
        get; set;
    } = new List<WorkflowLinkTarget>();
    [JsonIgnore]
    public bool IsLinkHoverStep { get; set; }
    [JsonIgnore]
    public bool IsHover { get; set; }

    public void Update(WorkflowLink link)
    {
        ActionType = link.ActionType;
        ActionParameter = link.ActionParameter;
    }

    public object Clone()
    {
        return new WorkflowLink
        {
            Id = Id,
            Source = Source.Clone(),
            SourceStepId = SourceStepId,
            ActionType = ActionType,
            ActionParameter = ActionParameter,
            IsMainLink = IsMainLink,
            Targets = Targets.Select(t => new WorkflowLinkTarget
            {
                Condition = t.Condition,
                TargetStepId = t.TargetStepId,
                Description = t.Description
            }).ToList()
        };
    }

    public bool IsLinked(string stepId)
    {
        return SourceStepId == stepId || Targets.Any(t => t.TargetStepId == stepId);
    }

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
