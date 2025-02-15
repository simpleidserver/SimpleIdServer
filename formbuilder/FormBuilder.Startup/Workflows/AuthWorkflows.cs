using FormBuilder.Link;
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

    public static WorkflowRecord SampleWorkflow = new WorkflowRecord
    {
        Id = "sampleWorkflow",
        Realm = Constants.DefaultRealm,
        UpdateDateTime = DateTime.UtcNow,
        Links = new List<WorkflowLink>
        {
            // Pwd => Console.
            new WorkflowLink
            {
                Id = Guid.NewGuid().ToString(),
                Source = new WorkflowLinkSource
                {
                    EltId = PwdAuthForms.authPwdFormId
                },
                SourceStepId = "pwdStep",
                TargetStepId = "mobilePwd",
                Description = "Authentication",
                ActionParameter = "{}",
                ActionType = WorkflowLinkHttpRequestAction.ActionType
            },
            // Pwd => Reset
            new WorkflowLink
            {
                Id = Guid.NewGuid().ToString(),
                Source = new WorkflowLinkSource
                {
                    EltId = PwdAuthForms.forgetMyPasswordId
                },
                SourceStepId = "pwdStep",
                TargetStepId = "forget",
                Description = "Reset",
                ActionParameter = "{}",
                ActionType = WorkflowLinkHttpRequestAction.ActionType
            },
        },
        Steps = new List<WorkflowStep>
        {
            new WorkflowStep
            {
                Id = "pwdStep",
                FormRecordCorrelationId = "pwd"
            },
            new WorkflowStep
            {
                Id = "mobilePwd",
                FormRecordCorrelationId = "mobileId"
            },
            new WorkflowStep
            {
                Id = "forget",
                FormRecordCorrelationId = "resetPwd"
            }
        }
    };
}
