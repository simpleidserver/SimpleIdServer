using FormBuilder.Conditions;
using FormBuilder.Models;

namespace FormBuilder.Builders;

public class WorkflowBuilder
{
    private readonly WorkflowRecord _workflow;
    private readonly List<WorkflowLinkBuilder> _workflowLinks;

    private WorkflowBuilder(WorkflowRecord workflow)
    {
        _workflow = workflow;
        _workflowLinks = new List<WorkflowLinkBuilder>();
    }

    public static WorkflowBuilder New(string id, string name = null)
    {
        return new WorkflowBuilder(new WorkflowRecord { Id = id, Realm = "master", Name = name });
    }

    public WorkflowBuilder AddStep(FormRecord record)
    {
        _workflow.Steps.Add(new WorkflowStep
        {
            FormRecordCorrelationId = record.CorrelationId,
            Id = Guid.NewGuid().ToString()
        });
        return this;
    }

    public WorkflowBuilder AddLink(FormRecord sourceForm, FormRecord targetForm, string eltId, string description, bool isMainLink, Action<WorkflowLink> cb = null)
    {
        _workflowLinks.Add(new WorkflowLinkBuilder(sourceForm, new List<(FormRecord form, IConditionParameter condition, string description)> { { (targetForm, null, description) } }, eltId, isMainLink, cb));
        return this;
    }

    public WorkflowBuilder AddLink(FormRecord sourceForm, List<(FormRecord form, IConditionParameter condition, string description)> targets, string eltId, bool isMainLink, Action<WorkflowLink> cb = null)
    {
        _workflowLinks.Add(new WorkflowLinkBuilder(sourceForm, targets, eltId, isMainLink, cb));
        return this;
    }

    public WorkflowRecord Build(DateTime currentDateTime)
    {
        foreach(var link in _workflowLinks)
        {
            var sourceStep = _workflow.GetStep(link.SourceForm.CorrelationId);
            var targets = new List<WorkflowLinkTarget>();
            foreach(var target in link.Targets)
            {
                WorkflowStep targetStep;
                if (target.form.CorrelationId == FormBuilder.Constants.EmptyStep.CorrelationId)
                {
                    targetStep = new WorkflowStep
                    {
                        Id = Guid.NewGuid().ToString(),
                        FormRecordCorrelationId = FormBuilder.Constants.EmptyStep.CorrelationId
                    };
                    _workflow.Steps.Add(targetStep);
                }
                else
                {
                    targetStep = _workflow.GetStep(target.form.CorrelationId);
                }

                var workflowLinkTarget = new WorkflowLinkTarget
                {
                    Condition = target.condition,
                    TargetStepId = targetStep.Id,
                    Description = target.description
                };
                targets.Add(workflowLinkTarget);
            }

            var workflowLink = new WorkflowLink
            {
                Id = Guid.NewGuid().ToString(),
                Source = new WorkflowLinkSource
                {
                    EltId = link.EltId
                },
                SourceStepId = sourceStep.Id,
                Targets = targets,
                IsMainLink = link.IsMainLink
            };
            _workflow.Links.Add(workflowLink);
            if (link.Cb != null) link.Cb(workflowLink);
        }

        return _workflow;
    }

    private record WorkflowLinkBuilder
    {
        public WorkflowLinkBuilder(FormRecord sourceForm, List<(FormRecord form, IConditionParameter condition, string description)> targets, string eltId, bool isMainLink, Action<WorkflowLink> cb)
        {
            SourceForm = sourceForm;
            Targets = targets;
            EltId = eltId;
            IsMainLink = isMainLink;
            Cb = cb;
        }

        public FormRecord SourceForm { get; private set; }
        public List<(FormRecord form, IConditionParameter condition, string description)> Targets { get; private set; }
        public string EltId { get; private set; }
        public bool IsMainLink { get; private set; }
        public Action<WorkflowLink> Cb { get; private set; }
    }
}
