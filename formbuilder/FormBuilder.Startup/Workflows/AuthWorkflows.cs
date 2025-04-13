using FormBuilder.Models;

namespace FormBuilder.Startup.Workflows;

public class AuthWorkflows
{
    public static WorkflowRecord SampleWorkflow = new WorkflowRecord
    {
        Id = "sampleWorkflow",
        Realm = Constants.DefaultRealm,
        UpdateDateTime = DateTime.UtcNow,
        Steps = new List<WorkflowStep>
        {
            new WorkflowStep
            {
                Id = "pwdStep",
                FormRecordCorrelationId = "pwd"
            }
        }
    };
}
