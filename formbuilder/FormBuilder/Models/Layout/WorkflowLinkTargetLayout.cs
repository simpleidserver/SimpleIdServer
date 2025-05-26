using FormBuilder.Conditions;

namespace FormBuilder.Models.Layout;

public class WorkflowLinkTargetLayout
{
    public string TargetFormCorrelationId 
    { 
        get; set; 
    }

    public IConditionParameter Condition
    { 
        get; set; 
    }

    public string Description
    {
        get; set;
    }
}