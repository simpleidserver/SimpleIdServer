using Blazor.Diagrams.Core.Models;

namespace FormBuilder.Components.Workflow;

public class WorkflowStepChooserNode : NodeModel
{
    public WorkflowStepChooserNode(Action<WorkflowStepChooserNode> act)
    {
        ActCb = act;
    }

    public WorkflowStepChooserNode(Action<WorkflowStepChooserNode> act, WorkflowStepNode node)
    {
        
    }

    public Action<WorkflowStepChooserNode> ActCb { get; private set; }
}