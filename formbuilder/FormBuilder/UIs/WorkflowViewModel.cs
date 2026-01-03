using FormBuilder.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FormBuilder.UIs;

public class WorkflowViewModel
{
    public WorkflowRecord Workflow { get; set; }
    public Template Template { get; set; }
    public List<FormRecord> FormRecords { get; set; }
    public JsonObject Input { get; set; } = new JsonObject();
    public AntiforgeryTokenRecord AntiforgeryToken { get; set; } = new AntiforgeryTokenRecord();
    public string CurrentStepId { get; set; }
    public string Realm { get; set; }
    public List<string> ErrorMessages { get; set; }
    public List<string> SuccessMessages { get; set; }

    public void SetErrorMessage(string errorMessage)
    {
        ErrorMessages = new List<string>
        {
            errorMessage
        };
    }

    public void SetErrorMessages(List<string> errorMessages)
    {
        ErrorMessages = errorMessages;
    }

    public void SetSuccessMessage(string successMessage)
    {
        SuccessMessages = new List<string>
        {
            successMessage
        };
    }

    public void SetSuccessMessages(List<string> successMessages)
    {
        SuccessMessages = successMessages;
    }

    public void SetInput<T>(T record) where T : class
    {
        Input = JsonObject.Parse(JsonSerializer.Serialize(record)).AsObject();
    }

    public void SetInput(object record)
        => Input = JsonObject.Parse(JsonSerializer.Serialize(record)).AsObject();

    public void StayCurrentStep(IStepViewModel viewModel)
    {
        var link = Workflow.Links.Single(l => l.Id == viewModel.CurrentLink);
        CurrentStepId = link.SourceStepId;
    }
}
