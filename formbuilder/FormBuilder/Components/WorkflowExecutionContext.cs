using FormBuilder.Models;
using System.Text.Json.Nodes;

namespace FormBuilder.Components;

public class WorkflowExecutionContext
{
    public WorkflowExecutionContext(FormRecord formRecord)
    {
        var currentStepId = Guid.NewGuid().ToString();
        Workflow = new WorkflowRecord
        {
            Steps = new List<WorkflowStep>
            {
                new WorkflowStep
                {
                    Id = currentStepId,
                    FormRecordName = formRecord.Name
                }
            }
        };
        Records = new List<FormRecord>
        {
            formRecord
        };
        CurrentStepId = currentStepId;
    }

    public WorkflowExecutionContext(WorkflowRecord workflow, List<FormRecord> records)
    {
        Workflow = workflow;
        Records = records;
    }

    public WorkflowExecutionContext(WorkflowRecord workflow, List<FormRecord> records, string currentStepId) : this(workflow, records)
    {
        CurrentStepId = currentStepId;
    }

    public WorkflowRecord Workflow { get; private set; }
    public List<FormRecord> Records { get; private set; }
    public string CurrentStepId { get; private set; }
    public JsonObject StepOutput { get; private set; }
    public AntiforgeryTokenRecord AntiforgeryToken { get; set; }

    public WorkflowLink GetLink(IFormElementRecord record)
    {
        var currentStep = GetCurrentStep();
        if (currentStep == null) return null;
        var link = Workflow.Links.SingleOrDefault(l => l.Source.EltId == record.Id);
        return link;
    }

    public void NextStep(WorkflowLink link)
        => CurrentStepId = link.TargetStepId;

    public FormRecord GetCurrentRecord()
    {
        var currentStep = GetCurrentStep();
        return Records.Single(r => r.Name == currentStep.FormRecordName);
    }

    public void SetStepOutput(JsonObject output)
        => StepOutput = output;

    public void SetAntifogeryToken(AntiforgeryTokenRecord antiforgeryToken)
        => AntiforgeryToken = antiforgeryToken;

    private WorkflowStep GetCurrentStep()
    {
        WorkflowStep currentStep = null;
        if (!string.IsNullOrWhiteSpace(CurrentStepId))
            currentStep = Workflow.Steps.Single(s => s.Id == CurrentStepId);
        else
            currentStep = Workflow.GetFirstStep();
        return currentStep;
    }
}
