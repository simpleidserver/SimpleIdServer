namespace FormBuilder.Components.Workflow;

public class WorkflowActionState
{
    private readonly Action _act;

    public WorkflowActionState(Action act)
    {
        _act = act;
    }

    public void Finish() => _act(); 
}
