using FormBuilder.Models;

namespace FormBuilder.Startup.Workflows;

public class AuthWorkflows
{
    public static WorkflowRecord AuthWorkflow = new WorkflowRecord
    {
        Id = Guid.NewGuid().ToString(),
        CorrelationId = "pwdMobile",
        UpdateDateTime = DateTime.UtcNow,
        Status = RecordVersionStatus.Draft,
        Realm = Constants.DefaultRealm,
        VersionNumber = 0
    };
}
