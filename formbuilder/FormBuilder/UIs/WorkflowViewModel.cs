using FormBuilder.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FormBuilder.UIs;

public class WorkflowViewModel
{
    public WorkflowRecord Workflow { get; set; }
    public List<FormRecord> FormRecords { get; set; }
    public JsonObject Input { get; set; }
    public AntiforgeryTokenRecord AntiforgeryToken { get; set; }
    public string CurrentStepId { get; set; }
    public string Realm { get; set; }
    public List<string> ErrorMessages { get; set; }
    public List<string> SuccessMessages { get; set; }

    public void SetErrorMessage(string errorMessage)
        => ErrorMessages = new List<string> { errorMessage };

    public void SetInput<T>(T record) where T : class
    {
        Input = JsonObject.Parse(JsonSerializer.Serialize(record)).AsObject();
    }
}
