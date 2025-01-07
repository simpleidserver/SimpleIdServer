using FormBuilder.Models;

namespace FormBuilder.Builders;

public class WorkflowBuilder
{
    private readonly WorkflowRecord _workflow;

    private WorkflowBuilder(WorkflowRecord workflow)
    {
        _workflow = workflow;
    }

    public static WorkflowBuilder New(string id)
    {
        return new WorkflowBuilder(new WorkflowRecord { Id = id });
    }

    public WorkflowBuilder AddStep(FormRecord record, Coordinate coordinate)
    {
        _workflow.Steps.Add(new WorkflowStep
        {
            Coordinate = coordinate,
            FormRecordId = record.Id,
            Id = Guid.NewGuid().ToString()
        });
        return this;
    }

    public WorkflowBuilder AddLink(FormRecord sourceForm, FormRecord targetForm, string eltId, Action<WorkflowLink> cb = null)
    {
        var sourceStep = _workflow.GetStep(sourceForm.Id);
        var targetStep = _workflow.GetStep(targetForm.Id);
        var workflowLink = new WorkflowLink
        {
            Id = Guid.NewGuid().ToString(),
            Source = new WorkflowLinkSource
            {
                EltId = eltId
            },
            SourceStepId = sourceStep.Id,
            TargetStepId = targetStep.Id
        };
        _workflow.Links.Add(workflowLink);
        if (cb != null) cb(workflowLink);
        return this;
    }

    public WorkflowRecord Build()
        => _workflow;
}
