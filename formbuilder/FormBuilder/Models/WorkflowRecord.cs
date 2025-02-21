using FormBuilder.Components.FormElements.ListData;

namespace FormBuilder.Models;

public class WorkflowRecord : ICloneable
{
    public string Id { get; set; }
    public string? Realm { get; set; }
    public DateTime UpdateDateTime { get; set; }
    public List<WorkflowStep> Steps { get; set; } = new List<WorkflowStep>();
    public List<WorkflowLink> Links { get; set; } = new List<WorkflowLink>();

    public List<WorkflowStep> GetAllChildrenMainLinks(string stepId)
    {
        var result = new List<WorkflowStep>();
        var link = Links.SingleOrDefault(l => l.SourceStepId == stepId && l.IsMainLink);
        if (link != null)
        {
            var targetStep = Steps.Single(s => s.Id == link.TargetStepId);
            if(!targetStep.IsEmptyStep)
            {
                result.Add(targetStep);
                result.AddRange(GetAllChildrenMainLinks(link.TargetStepId));
            }
        }

        return result;
    }

    public List<WorkflowStep> GetAllParentMainLinks(string stepId)
    {
        var result = new List<WorkflowStep>();
        var link = Links.SingleOrDefault(l => l.TargetStepId == stepId);
        if (link != null)
        {
            var sourceStep = Steps.Single(s => s.Id == link.SourceStepId);
            if (!sourceStep.IsEmptyStep)
            {
                result.Add(sourceStep);
                result.AddRange(GetAllParentMainLinks(link.SourceStepId));
            }
        }

        result.Reverse();
        return result;
    }

    public int ComputeLevel(WorkflowStep step)
    {
        if (step.FormRecordCorrelationId == Constants.EmptyStep.CorrelationId) return 999;
        var link = Links.SingleOrDefault(l => l.TargetStepId == step.Id);
        if (link == null) return 0;
        var parentStep = Steps.Single(s => s.Id == link.SourceStepId);
        return ComputeLevel(parentStep) + 1;
    }

    public object Clone()
    {
        return new WorkflowRecord
        {
            Id = Id,
            Realm = Realm,
            UpdateDateTime = UpdateDateTime,
            Steps = Steps.Select(s => s.Clone() as WorkflowStep).ToList(),
            Links = Links.Select(s => s.Clone() as WorkflowLink).ToList()
        };
    }

    public WorkflowStep GetStep(string id)
        => Steps.SingleOrDefault(s => s.FormRecordCorrelationId == id);

    public void Update(List<WorkflowStep> steps, List<WorkflowLink> links, DateTime currentDateTime)
    {
        var unknownSteps = steps.Where(s => !Steps.Any(es => es.Id == s.Id)).ToList();
        Steps = Steps.Where(es => steps.Any(s => s.Id == es.Id)).ToList();
        Steps.AddRange(unknownSteps);

        var unknownLinks = links.Where(l => !Links.Any(el => el.Id == l.Id)).ToList();
        Links = Links.Where(el => links.Any(l => l.Id == el.Id)).ToList();
        foreach (var link in Links) link.Update(links.Single(l => l.Id == link.Id));
        Links.AddRange(unknownLinks);
        UpdateDateTime = currentDateTime;
    }

    public WorkflowStep GetFirstStep()
    {
        var targetStepIds = Links.Select(l => l.TargetStepId);
        var filteredSteps = Steps.Where(s => !targetStepIds.Contains(s.Id));
        return filteredSteps.FirstOrDefault();
    }

    public void AddLink(WorkflowLink workflowLink)
        => Links.Add(workflowLink);

    public (FormRecord form, ListDataRecord formElt) GetListDataRecord(WorkflowLink workflowLink, List<FormRecord> forms)
    {
        var record = GetElementRecord(workflowLink, forms);
        return (record.form, record.formElt as ListDataRecord);
    }

    public (FormRecord form, IFormElementRecord formElt) GetElementRecord(WorkflowLink workflowLink, List<FormRecord> forms)
    {
        var workflowStep = Steps.Single(f => f.Id == workflowLink.SourceStepId);
        var form = forms.Single(f => f.CorrelationId == workflowStep.FormRecordCorrelationId);
        return (form, form.GetChild(workflowLink.Source.EltId));
    }

    public List<WorkflowLink> GetLinks(WorkflowStep step)
        => Links.Where(l => l.IsLinked(step.Id)).ToList();
}