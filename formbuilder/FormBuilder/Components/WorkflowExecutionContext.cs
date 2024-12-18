using FormBuilder.Models;
using Microsoft.AspNetCore.Components;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FormBuilder.Components;

public class WorkflowContext
{
    private WorkflowContext() {  }

    public WorkflowDefinition Definition { get; private set; }
    public WorkflowExecution Execution { get; private set; }
    public FormEditorContext FormEditorContext { get; private set; }
    public WorkflowEditorContext WorkflowEditorContext { get; private set; }

    public static WorkflowContext CreateOneStepWorkflow(FormRecord form)
    {
        var currentStepId = Guid.NewGuid().ToString();
        var workflow = new WorkflowRecord
        {
            Steps = new List<WorkflowStep>
            {
                new WorkflowStep
                {
                    Id = currentStepId,
                    FormRecordName = form.Name
                }
            }
        };
        var records = new List<FormRecord>
        {
            form
        };
        var result = new WorkflowContext
        {
            Definition = new WorkflowDefinition(workflow, records),
            Execution = new WorkflowExecution { CurrentStepId = currentStepId },
            FormEditorContext = new FormEditorContext(),
            WorkflowEditorContext = new WorkflowEditorContext()
        };
        return result;
    }

    #region Getters

    public FormRecord GetCurrentFormRecord()
    {
        var currentStep = GetCurrentStepDefinition();
        return Definition.Records.Single(r => r.Name == currentStep.FormRecordName);
    }

    public JsonObject GetCurrentStepInputData()
    {
        var stepExecution = GetCurrentStepExecution();
        return stepExecution?.InputData;
    }

    public WorkflowLink GetLinkDefinitionFromCurrentStep(string eltId)
    {
        var executionLink = GetLinkExecutionFromElementAndCurrentStep(eltId);
        if (executionLink == null) return null;
        return Definition.Workflow.Links.SingleOrDefault(l => l.Source.EltId == eltId);
    }

    public WorkflowStepLinkExecution GetLinkExecutionFromCurrentStep(string id)
    {
        var stepExecution = GetCurrentStepExecution();
        return stepExecution.Links.SingleOrDefault(l => l.Id == id);
    }

    public WorkflowStepLinkExecution GetLinkExecutionFromElementAndCurrentStep(string eltId)
    {
        var stepExecution = GetCurrentStepExecution();
        return stepExecution.Links.SingleOrDefault(l => l.EltId == eltId);
    }

    public WorkflowStepExecution GetCurrentStepExecution()
        => Execution.StepExecutions.SingleOrDefault(e => e.StepId == Execution.CurrentStepId);

    public WorkflowStep GetCurrentStepDefinition()
        => Definition.Workflow.Steps.Single(s => s.Id == Execution.CurrentStepId);

    #endregion

    #region Actions

    public void NextStep(WorkflowLink link)
    {
        Execution.CurrentStepId = link.TargetStepId;
    }

    #endregion
}

public class WorkflowExecution
{
    public string CurrentStepId { get; set; }
    public AntiforgeryTokenRecord AntiforgeryToken { get; set; }
    public List<WorkflowStepExecution> StepExecutions { get; set; } = new List<WorkflowStepExecution>();
}

public class WorkflowStepExecution
{
    public string StepId { get; set; }
    public JsonObject InputData { get; set; }
    public List<WorkflowStepLinkExecution> Links { get; set; } = new List<WorkflowStepLinkExecution>();
}

public class WorkflowStepLinkExecution
{
    public string Id { get; set; }
    public string LinkId { get; set; }
    public string EltId { get; set; }
    public JsonNode InputData { get; set; }
    public JsonObject OutputData { get; set; }
}

public class WorkflowDefinition
{
    public WorkflowDefinition(WorkflowRecord workflow, List<FormRecord> records)
    {
        Workflow = workflow;
        Records = records;
    }

    public WorkflowRecord Workflow { get; private set; }
    public List<FormRecord> Records { get; private set; }
}

public class FormEditorContext
{
    private Action _droppedCallback;
    public SelectionTypes SelectionType { get; private set; }
    public IFormElementRecord SelectedEltInstance { get; private set; }
    public IFormElementDefinition SelectedEltDefinition { get; private set; }

    public void SelectInstance(IFormElementRecord selectedEltInstance, Action droppedCallback)
    {
        SelectionType = SelectionTypes.RECORD;
        SelectedEltInstance = selectedEltInstance;
        SelectedEltDefinition = null;
    }

    public void SelectDefinition(IFormElementDefinition def)
    {
        SelectionType = SelectionTypes.DEFINITION;
        SelectedEltDefinition = def;
        SelectedEltInstance = null;
    }

    public void DropElement()
    {
        SelectedEltDefinition = null;
        SelectedEltInstance = null;
        if (_droppedCallback != null) _droppedCallback();
    }
}

public enum SelectionTypes
{
    DEFINITION = 0,
    RECORD = 1
}

public class WorkflowEditorContext
{
    public event EventHandler<FormEltEventArgs> WorkflowLinkChanged;

    public void StartDragElt(ElementReference eltReference, IFormElementRecord record)
    {
        WorkflowLinkChanged(this, new FormEltEventArgs(eltReference, record));
    }
}

public class FormEltEventArgs : EventArgs
{
    public FormEltEventArgs(ElementReference eltReference, IFormElementRecord record)
    {
        EltReference = eltReference;
        Record = record;
    }

    public ElementReference EltReference { get; private set; }
    public IFormElementRecord Record { get; private set; }
}

public class WorkflowExecutionContext
{
    public WorkflowExecutionContext()
    {
        
    }

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
    public JsonNode RepetitionRuleData { get; set; }
    public JsonObject InputData { get; set; }

    public WorkflowExecutionContext Clone()
    {
        return new WorkflowExecutionContext()
        {
            AntiforgeryToken = AntiforgeryToken,
            CurrentStepId = CurrentStepId,
            Records = Records,
            StepOutput = StepOutput,
            Workflow = JsonSerializer.Deserialize<WorkflowRecord>(JsonSerializer.Serialize(Workflow))
        };
    }

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