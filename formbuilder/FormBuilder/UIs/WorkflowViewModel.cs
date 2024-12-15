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
    public List<string> ErrorMessages { get; set; }

    public void SetInput<T>(T record) where T : class
    {
        Input = JsonObject.Parse(JsonSerializer.Serialize(record)).AsObject();
    }
}
