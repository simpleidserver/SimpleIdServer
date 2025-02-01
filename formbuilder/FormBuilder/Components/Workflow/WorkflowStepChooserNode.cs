using Blazor.Diagrams.Core.Models;
using FormBuilder.Models.Layout;

namespace FormBuilder.Components.Workflow;

public class WorkflowStepChooserNode : NodeModel
{
    public WorkflowStepChooserNode(Action<WorkflowStepChooserNode, WorkflowStepChooserRecord> act, List<WorkflowLayout> workflowLayouts)
    {
        ActCb = act;
        Records = workflowLayouts.Select(l => new WorkflowStepChooserRecord(l)).ToList();
        Records.Add(WorkflowStepChooserRecord.NewEndStep());
    }

    public Action<WorkflowStepChooserNode, WorkflowStepChooserRecord> ActCb { get; private set; }
    public List<WorkflowStepChooserRecord> Records { get; set; }
}

public class WorkflowStepChooserRecord
{
    public WorkflowStepChooserRecord()
    {
        
    }

    public WorkflowStepChooserRecord(WorkflowLayout workflowLayout)
    {
        Id = Guid.NewGuid().ToString();
        Name = workflowLayout.Name;
        WorkflowCorrelationId = workflowLayout.WorkflowCorrelationId;
        IsEndStep = false;
    }

    public string Id { get; set; }
    public string WorkflowCorrelationId { get; set; }
    public string Name { get; set; }
    public bool IsEndStep { get; set; }

    public static WorkflowStepChooserRecord NewEndStep()
    {
        return new WorkflowStepChooserRecord
        {
            Id = Guid.NewGuid().ToString(),
            Name = "End Step",
            IsEndStep = true
        };
    }
}