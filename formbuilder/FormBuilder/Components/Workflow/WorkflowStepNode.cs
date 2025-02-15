using Blazor.Diagrams.Core.Models;
using FormBuilder.Models;

namespace FormBuilder.Components.Workflow;

public class WorkflowStepNode : NodeModel
{
    public WorkflowStepNode(WorkflowStep step, FormRecord form, bool isBlocked, Action<WorkflowStepNode> editCb)
    {
        Step = step;
        Form = form;
        IsBlocked = isBlocked;
        EditCb = editCb;
    }

    public WorkflowStep Step { get; set; }
    public FormRecord Form { get; set; }
    public bool IsBlocked { get; set; }
    public Action<WorkflowStepNode> EditCb { get; set; }
}