﻿using FormBuilder.Models;

namespace FormBuilder.Builders;

public class WorkflowBuilder
{
    private readonly WorkflowRecord _workflow;
    private readonly List<WorkflowLinkBuilder> _workflowLinks;

    private WorkflowBuilder(WorkflowRecord workflow)
    {
        _workflow = workflow;
        _workflowLinks = new List<WorkflowLinkBuilder>();
        AddStep(Constants.EmptyStep);
    }

    public static WorkflowBuilder New(string id, string correlationId)
    {
        return new WorkflowBuilder(new WorkflowRecord { Id = id, CorrelationId = id });
    }

    public WorkflowBuilder AddStep(FormRecord record)
    {
        _workflow.Steps.Add(new WorkflowStep
        {
            FormRecordId = record.Id,
            Id = Guid.NewGuid().ToString()
        });
        return this;
    }

    public WorkflowBuilder AddLink(FormRecord sourceForm, FormRecord targetForm, string eltId, Action<WorkflowLink> cb = null)
    {
        _workflowLinks.Add(new WorkflowLinkBuilder(sourceForm, targetForm, eltId, cb));
        return this;
    }

    public WorkflowRecord Build(DateTime currentDateTime)
    {
        foreach(var link in _workflowLinks)
        {
            var sourceStep = _workflow.GetStep(link.SourceForm.Id);
            var targetStep = _workflow.GetStep(link.TargetForm.Id);
            var workflowLink = new WorkflowLink
            {
                Id = Guid.NewGuid().ToString(),
                Source = new WorkflowLinkSource
                {
                    EltId = link.EltId
                },
                SourceStepId = sourceStep.Id,
                TargetStepId = targetStep.Id
            };
            _workflow.Links.Add(workflowLink);
            if (link.Cb != null) link.Cb(workflowLink);
        }

        _workflow.Publish(currentDateTime);
        return _workflow;
    }

    private record WorkflowLinkBuilder
    {
        public WorkflowLinkBuilder(FormRecord sourceForm, FormRecord targetForm, string eltId, Action<WorkflowLink> cb)
        {
            SourceForm = sourceForm;
            TargetForm = targetForm;
            EltId = eltId;
            Cb = cb;
        }

        public FormRecord SourceForm { get; private set; }
        public FormRecord TargetForm { get; private set; }
        public string EltId { get; private set; }
        public Action<WorkflowLink> Cb { get; private set; }
    }
}
