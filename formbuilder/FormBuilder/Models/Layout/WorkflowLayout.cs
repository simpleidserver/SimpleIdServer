namespace FormBuilder.Models.Layout;

public class WorkflowLayout
{
    public string WorkflowCorrelationId { get; set; }
    public string SourceFormCorrelationId { get; set; }
    public string Name { get; set; }
    public List<WorkflowLinkLayout> Links { get; set; }
}