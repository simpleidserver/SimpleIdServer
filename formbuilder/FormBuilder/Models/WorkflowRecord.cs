using FormBuilder.Components.FormElements.ListData;

namespace FormBuilder.Models;

public class WorkflowRecord : BaseVersionRecord
{
    public string Id { get; set; }
    public string? Realm { get; set; }
    public List<WorkflowStep> Steps { get; set; } = new List<WorkflowStep>();
    public List<WorkflowLink> Links { get; set; } = new List<WorkflowLink>();

    public WorkflowStep GetStep(string id)
        => Steps.SingleOrDefault(s => s.FormRecordId == id);

    public void Update(List<WorkflowStep> steps, List<WorkflowLink> links, DateTime currentDateTime)
    {
        Steps = steps;
        Links = links;
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
        var form = forms.Single(f => f.Id == workflowStep.FormRecordId);
        return (form, form.GetChild(workflowLink.Source.EltId));
    }

    public List<WorkflowLink> GetLinks(WorkflowStep step)
        => Links.Where(l => l.IsLinked(step.Id)).ToList();

    public override BaseVersionRecord NewDraft(DateTime currentDateTime)
    {
        var steps = Steps.Select(s => (WorkflowStep)s.Clone()).ToList();
        var links = Links.Select(s => (WorkflowLink)s.Clone()).ToList();
        steps.ForEach(s => s.Id = Guid.NewGuid().ToString());
        links.ForEach(s => s.Id = Guid.NewGuid().ToString());
        return new WorkflowRecord
        {
            Id = Guid.NewGuid().ToString(),
            CorrelationId = CorrelationId,
            Steps = steps,
            Links = links,
            UpdateDateTime = UpdateDateTime,
            Status = RecordVersionStatus.Draft,
            VersionNumber = VersionNumber + 1
        };
    }
}