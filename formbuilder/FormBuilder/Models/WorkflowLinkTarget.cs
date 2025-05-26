using FormBuilder.Conditions;

namespace FormBuilder.Models;

public class WorkflowLinkTarget
{
    public IConditionParameter Condition
    {
        get; set;
    }

    public string TargetStepId
    {
        get; set;
    }

    public string Description
    {
        get; set;
    }
}
