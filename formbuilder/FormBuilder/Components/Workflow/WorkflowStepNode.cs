using Blazor.Diagrams.Core.Models;
using FormBuilder.Models;

namespace FormBuilder.Components.Workflow;

public class WorkflowStepNode : NodeModel
{
    public WorkflowStepNode(WorkflowStep step, FormRecord form)
    {
        Step = step;
        Form = form;
    }

    public WorkflowStep Step { get; set; }
    public FormRecord Form { get; set; }
}