namespace FormBuilder.Models.Layout;

public class WorkflowLinkLayout
{
    public string EltCorrelationId { get; set; }
    public List<WorkflowLinkTargetLayout> Targets
    {
        get; set;
    }
    public string ActionType { get; set; }
    public string? ActionParameter { get; set; }
    public bool IsMainLink { get; set; }
}