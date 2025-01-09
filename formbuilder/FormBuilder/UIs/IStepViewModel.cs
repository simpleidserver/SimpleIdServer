namespace FormBuilder.UIs;

public interface IStepViewModel
{
    string StepId { get; set; }
    string WorkflowId { get; set; }
    string CurrentLink { get; set; }
}