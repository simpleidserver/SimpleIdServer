using FormBuilder.Builders;
using FormBuilder.Models;
using SimpleIdServer.IdServer.Pwd;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Host.Acceptance.Tests;

public class FormBuilderConfiguration
{
    public static List<FormRecord> allForms = new List<FormRecord>
    {
        StandardPwdAuthForms.PwdForm,
        StandardPwdAuthForms.ResetForm,
        StandardPwdAuthForms.ConfirmResetForm,
        StandardPwdRegisterForms.PwdForm
    };

    public static List<WorkflowRecord> allWorkflows = new List<WorkflowRecord>
    {
        StandardPwdAuthWorkflows.DefaultPwdWorkflow,
        StandardPwdAuthWorkflows.DefaultConfirmResetPwdWorkflow,
        StandardPwdRegistrationWorkflows.DefaultWorkflow
    };
}
