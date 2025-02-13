using FormBuilder.Models;

namespace FormBuilder.Startup.Workflows;

public class AuthWorkflows
{
    public static WorkflowRecord AuthWorkflow = new WorkflowRecord
    {
        Id = "workflow",
        UpdateDateTime = DateTime.UtcNow,
        Realm = Constants.DefaultRealm
    };
}
