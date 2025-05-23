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
        _workflowLinks.Add(new WorkflowLinkBuilder(sourceForm, targetForm, eltId, description, isMainLink, cb));
        return this;
    }

    public WorkflowRecord Build(DateTime currentDateTime)
    {
        foreach(var link in _workflowLinks)
        {
            var sourceStep = _workflow.GetStep(link.SourceForm.CorrelationId);
            WorkflowStep targetStep = null;
            if(link.TargetForm.CorrelationId == FormBuilder.Constants.EmptyStep.CorrelationId)
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
                targetStep = _workflow.GetStep(link.TargetForm.CorrelationId);
            }

            var workflowLink = new WorkflowLink
            {
                Id = Guid.NewGuid().ToString(),
                Source = new WorkflowLinkSource
                {
                    EltId = link.EltId
                },
                SourceStepId = sourceStep.Id,
                TargetStepId = targetStep.Id,
                Description = link.Description,
                IsMainLink = link.IsMainLink
            };
            _workflow.Links.Add(workflowLink);
            if (link.Cb != null) link.Cb(workflowLink);
        }

        return _workflow;
    }

    private record WorkflowLinkBuilder
    {
        public WorkflowLinkBuilder(FormRecord sourceForm, FormRecord targetForm, string eltId, string description, bool isMainLink, Action<WorkflowLink> cb)
        {
            SourceForm = sourceForm;
            TargetForm = targetForm;
            EltId = eltId;
            Description = description;
            IsMainLink = isMainLink;
            Cb = cb;
        }

        public FormRecord SourceForm { get; private set; }
        public FormRecord TargetForm { get; private set; }
        public string EltId { get; private set; }
        public string Description { get; private set; }
        public bool IsMainLink { get; private set; }
        public Action<WorkflowLink> Cb { get; private set; }
    }
}
