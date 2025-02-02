using Blazor.Diagrams.Core.Models;
using FormBuilder.Models;

namespace FormBuilder.Components.Workflow;

public class WorkflowStepNode : NodeModel
{
    public WorkflowStepNode(WorkflowStep step, FormRecord form, Action<WorkflowStepNode> editCb)
    {
        Step = step;
        Form = form;
        EditCb = editCb;
    }

    public WorkflowStep Step { get; set; }
    public FormRecord Form { get; set; }
    public Action<WorkflowStepNode> EditCb { get; set; }
}