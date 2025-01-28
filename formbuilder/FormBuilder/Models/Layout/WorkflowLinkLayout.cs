namespace FormBuilder.Models.Layout;

public class WorkflowLinkLayout
{
    public string EltCorrelationId { get; set; }
    public string TargetFormCorrelationId { get; set; }
    public string ActionType { get; set; }
    public string? ActionParameter { get; set; }
}
