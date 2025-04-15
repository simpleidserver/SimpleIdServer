using FormBuilder.Models;
using FormBuilder.Models.Layout;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FormBuilder.Components;

public class WorkflowContext
{
    private WorkflowContext() {  }

    public WorkflowDefinition Definition { get; private set; }
    public WorkflowExecution Execution { get; private set; }
    public FormEditorContext FormEditorContext { get; private set; }
    public Template Template{ get; set; }
    public List<string> FilteredJsonPath { get; set; } = new List<string>();

    public static WorkflowContext CreateEmptyWorkflow(List<FormRecord> records)
    {
        return new WorkflowContext
        {
            Definition = new WorkflowDefinition(new WorkflowRecord(), records),
            Execution = new WorkflowExecution(),
            FormEditorContext = new FormEditorContext(null)
        };
    }

    public static WorkflowContext CreateOneStepWorkflow(FormRecord form, WorkflowLayout layout = null, Template template = null)
    {
        var currentStepId = Guid.NewGuid().ToString();
        var workflow = new WorkflowRecord
        {
            Steps = new List<WorkflowStep>
            {
                new WorkflowStep
                {
                    Id = currentStepId,
                    FormRecordCorrelationId = form.CorrelationId
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
            Execution = new WorkflowExecution(currentStepId),
            FormEditorContext = new FormEditorContext(layout),
            Template = template
        };
        return result;
    }

    public static WorkflowContext CreateOneStepWorkflow(FormRecord form, JsonObject inputData)
    {
        var result = CreateOneStepWorkflow(form);
        InitializeWorkflowExecution(result, inputData);
        return result;
    }

    public static WorkflowContext CreateWorkflow(WorkflowRecord workflow, List<FormRecord> forms)
    {
        var result = new WorkflowContext
        {
            Definition = new WorkflowDefinition(workflow, forms),
            Execution = new WorkflowExecution(),
            FormEditorContext = new FormEditorContext(null)
        };
        return result;
    }

    public static WorkflowContext CreateWorkflow(WorkflowRecord workflow, List<FormRecord> records, string currentStepId, List<string> errorMessages, List<string> successMessages, AntiforgeryTokenRecord antiforgeryTokenRecord, JsonObject inputData, List<string> supportedLanguageCodes, Template template)
    {
        var clonedWorkflow = workflow.Clone() as WorkflowRecord;
        var clonedRecords = records.Select(x => x.Clone() as FormRecord).ToList();
        var result = new WorkflowContext
        {
            Template = template,
            Definition = new WorkflowDefinition(clonedWorkflow, clonedRecords),
            Execution = new WorkflowExecution(currentStepId, errorMessages, successMessages, antiforgeryTokenRecord, supportedLanguageCodes),
            FormEditorContext = new FormEditorContext(null)
        };
        InitializeWorkflowExecution(result, inputData);
        return result;
    }

    #region Getters

    public FormRecord GetCurrentFormRecord()
    {
        var currentStep = GetCurrentStepDefinition();
        if (currentStep == null) return null;
        return Definition.Records.Single(r => r.CorrelationId == currentStep.FormRecordCorrelationId);
    }

    public IFormElementRecord GetFormRecord(string eltId)
        => Definition.Records.Select(r => r.GetChild(eltId)).FirstOrDefault(r => r != null);

    public JsonObject GetCurrentStepInputData()
    {
        var stepExecution = GetCurrentStepExecution();
        return stepExecution?.InputData;
    }

    public WorkflowLink GetLinkDefinitionFromCurrentStep(string eltId)
    {
        var executionLink = GetLinkExecutionFromElementAndCurrentStep(eltId);
        if (executionLink == null) return null;
        return Definition.Workflow.Links.SingleOrDefault(l => l.Id == executionLink.LinkId);
    }

    public WorkflowStepLinkExecution GetLinkExecutionFromCurrentStep(string id)
    {
        var stepExecution = GetCurrentStepExecution();
        return stepExecution.Links.SingleOrDefault(l => l.Id == id);
    }

    public WorkflowStepLinkExecution GetLinkExecutionFromFormEltAndCurrentStep(string eltId)
    {
        var stepExecution = GetCurrentStepExecution();
        return stepExecution.Links.SingleOrDefault(l => l.EltId == eltId);
    }

    public WorkflowStepLinkExecution GetLinkExecutionFromElementAndCurrentStep(string eltId)
    {
        var stepExecution = GetCurrentStepExecution();
        return stepExecution?.Links?.SingleOrDefault(l => l.EltId == eltId);
    }

    public WorkflowStepLinkExecution GetLinkExecution(string id)
        => Execution.StepExecutions.SelectMany(s => s.Links).SingleOrDefault(l => l.Id == id);

    public WorkflowStepExecution GetCurrentStepExecution()
        => Execution.StepExecutions.SingleOrDefault(e => e.StepId == Execution.CurrentStepId);

    public WorkflowStep GetCurrentStepDefinition()
        => GetStepDefinition(Execution.CurrentStepId);

    public WorkflowStep GetStepDefinition(string stepId)
        => Definition.Workflow?.Steps?.SingleOrDefault(s => s.Id == stepId);

    #endregion

    #region Actions

    public WorkflowStep GetFirstStep()
    {
        var allLinkedSteps = Definition.Workflow.Links.Select(l => l.TargetStepId);
        var firstStep = Definition.Workflow.Steps.FirstOrDefault(s => !allLinkedSteps.Contains(s.Id));
        return firstStep;
    }

    public void NavigateToFirstStep()
    {
        var firstStep = GetFirstStep();
        if (firstStep == null) return;
        Execution.CurrentStepId = firstStep.Id;
    }

    public void NextStep(WorkflowLink link)
        => NavigateToStep(link.TargetStepId);

    public void NavigateToStep(string stepId)
    {
        Execution.CurrentStepId = stepId;
        if(Execution.CurrentStepIdChanged != null) Execution.CurrentStepIdChanged(Execution, new EventArgs());
    }

    public WorkflowContext BuildContextAndMoveToStep(string stepId)
    {
        var result = new WorkflowContext
        {
            FormEditorContext = new FormEditorContext(null),
            Definition = Definition,
            Execution = new WorkflowExecution
            {
                StepExecutions = Execution.StepExecutions,
                AntiforgeryToken = Execution.AntiforgeryToken
            }
        };
        result.NavigateToStep(stepId);
        return result;
    }

    public WorkflowContext BuildEmptyContextAndMoveToStep(string stepId)
    {
        return new WorkflowContext
        {
            FormEditorContext = new FormEditorContext(null),
            Definition = Definition,
            Execution= new WorkflowExecution(stepId)
        };
    }

    public WorkflowContext BuildNewContextAndMoveToFirstStep(JsonObject input)
    {
        var result = new WorkflowContext
        {
            FormEditorContext = new FormEditorContext(null),
            Definition = Definition,
            Execution = new WorkflowExecution()
        };
        result.NavigateToFirstStep();
        InitializeWorkflowExecution(result, input);
        return result;
    }

    public WorkflowContext BuildNewContext(JsonObject input)
    {
        var result = new WorkflowContext
        {
            FormEditorContext = new FormEditorContext(null),
            Definition = Definition,
            Execution = new WorkflowExecution { CurrentStepId = Execution.CurrentStepId }
        };
        InitializeWorkflowExecution(result, input);
        return result;
    }

    private static void InitializeWorkflowExecution(WorkflowContext context, JsonObject jsonObject)
    {
        if (context.Definition == null || context.Definition.Workflow == null) return;
        var links = new List<WorkflowStepLinkExecution>();
        var currentStepLinks = context.Definition.Workflow.Links.Where(l => l.SourceStepId == context.Execution.CurrentStepId);
        context.Execution.StepExecutions.Add(new WorkflowStepExecution
        {
            InputData = jsonObject,
            Links = currentStepLinks.Select(l => new WorkflowStepLinkExecution
            {
                Id = Guid.NewGuid().ToString(),
                EltId = l.Source.EltId,
                InputData = jsonObject,
                OutputData = jsonObject,
                LinkId = l.Id
            }).ToList(),
            StepId = context.Execution.CurrentStepId
        });
    }

    #endregion
}

public class WorkflowExecution
{
    public WorkflowExecution()
    {
        
    }

    public WorkflowExecution(string currentStepId)
    {
        CurrentStepId = currentStepId;
    }

    public WorkflowExecution(string currentStepId, List<string> errorMessages, List<string> successMessages, AntiforgeryTokenRecord antiforgeryTokenRecord, List<string> supportedLanguageCodes)  : this(currentStepId)
    {
        ErrorMessages = errorMessages;
        SuccessMessages = successMessages;
        AntiforgeryToken = antiforgeryTokenRecord;
        SupportedLanguageCodes = supportedLanguageCodes;
    }

    public string CurrentStepId { get; set; }
    public EventHandler<EventArgs> CurrentStepIdChanged { get; set; }
    public List<string> ErrorMessages { get; set; }
    public List<string> SuccessMessages { get; set; }
    public AntiforgeryTokenRecord AntiforgeryToken { get; set; }
    public List<WorkflowStepExecution> StepExecutions { get; set; } = new List<WorkflowStepExecution>();
    public List<string> SupportedLanguageCodes { get; set; }
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
    public JsonNode OutputData { get; set; }
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
    private readonly WorkflowLayout _workflowLayout;
    private Action _droppedCallback;
    public SelectionTypes SelectionType { get; private set; }
    public IFormElementRecord SelectedEltInstance { get; private set; }
    public IFormElementDefinition SelectedEltDefinition { get; private set; }

    public FormEditorContext(WorkflowLayout workflowLayout)
    {
        _workflowLayout = workflowLayout;
    }

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

    public bool IsEltInvolvedInWorkflow<TRecord>(TRecord record) where TRecord : IFormElementRecord
    {
        if (_workflowLayout == null) return false;
        var workflowCorrelationIds = _workflowLayout.Links.Select(l => l.EltCorrelationId);
        return workflowCorrelationIds.Contains(record.CorrelationId);
    }
}

public enum SelectionTypes
{
    DEFINITION = 0,
    RECORD = 1
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
                    FormRecordCorrelationId = formRecord.CorrelationId
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
        return Records.Single(r => r.CorrelationId == currentStep.FormRecordCorrelationId);
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