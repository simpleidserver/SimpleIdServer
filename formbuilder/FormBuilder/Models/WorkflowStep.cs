namespace FormBuilder.Models;

public class WorkflowStep
{
    public string Id { get; set; }
    public string FormRecordName { get; set; }
    public Coordinate Coordinate { get; set; }
    public Size Size { get; set; }    
}