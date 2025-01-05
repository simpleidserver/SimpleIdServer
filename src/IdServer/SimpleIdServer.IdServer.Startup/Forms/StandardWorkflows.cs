using FormBuilder.Builders;
using FormBuilder.Link;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Transformers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SimpleIdServer.IdServer.Startup.Forms;

public class StandardWorkflows
{
    public static string defaultWorkflowId = "241a7509-4c58-4f49-b1df-49011b2c9bcb";
    public static string pwdConsoleWorkflowId = "e7593fa9-5a73-41a3-bfb5-e489fabbe17a";
    public static string pwdEmailWorkflowId = "62cb8fcc-34b6-4af9-8d54-db1c98827a08";
    public static string pwdSmsWorkflowId = "08bea90f-2183-4c56-977f-fd0a9c5e32b8";

    public static WorkflowRecord DefaultWorkflow = WorkflowBuilder.New(defaultWorkflowId)
        .AddStep(StandardForms.LoginPwdAuthForm, new Coordinate(100, 100))
        .AddStep(StandardForms.ResetPwdForm, new Coordinate(200, 100))
        .AddStep(FormBuilder.Constants.EmptyStep, new Coordinate(400, 100))
        .AddLinkHttpRequestAction(StandardForms.LoginPwdAuthForm, FormBuilder.Constants.EmptyStep, StandardForms.pwdAuthFormId, new WorkflowLinkHttpRequestParameter
        {
            Method = HttpMethods.POST,
            IsAntiforgeryEnabled = true,
            Target = "https://localhost:5001/{realm}/pwd/Authenticate",
            TargetTransformer = new RegexTransformerParameters()
            {
                Rules = new ObservableCollection<MappingRule>
                {
                    new MappingRule { Source = "$.Realm", Target = "realm" }
                }
            }
        })
        .AddLinkHttpRequestAction(StandardForms.LoginPwdAuthForm, StandardForms.ResetPwdForm, StandardForms.pwdForgetBtnId, new WorkflowLinkHttpRequestParameter
        {
            Method = HttpMethods.GET,
            TargetTransformer = new RegexTransformerParameters()
            {
                Rules = new ObservableCollection<MappingRule>
                {
                    new MappingRule { Source = "$.ReturnUrl", Target = "returnUrl" },
                    new MappingRule { Source = "$.Realm", Target = "realm" }
                }
            },
            IsCustomParametersEnabled = true,
            Rules = new ObservableCollection<MappingRule>
            {

            },
            Target = "https://localhost:5001/{realm}/pwd/Reset?returnUrl={returnUrl}"
        })
        .AddLinkHttpRequestAction(StandardForms.ResetPwdForm, FormBuilder.Constants.EmptyStep, StandardForms.pwdResetFormId, new WorkflowLinkHttpRequestParameter
        {
            Method = HttpMethods.POST,
            IsAntiforgeryEnabled = true,
            Target = "https://localhost:5001/{realm}/pwd/Reset",
            TargetTransformer = new RegexTransformerParameters()
            {
                Rules = new ObservableCollection<MappingRule>
                {
                    new MappingRule { Source = "$.Realm", Target = "realm" }
                }
            }
        })
        .Build();

    public static WorkflowRecord PwdConsoleWorkflow = WorkflowBuilder.New(pwdConsoleWorkflowId)
        .AddStep(StandardForms.LoginPwdAuthForm, new Coordinate(100, 100))
        .AddStep(StandardForms.ConsoleAuthForm, new Coordinate(200, 100))
        .AddStep(FormBuilder.Constants.EmptyStep, new Coordinate(300, 100))
        .AddLinkHttpRequestAction(StandardForms.LoginPwdAuthForm, StandardForms.ConsoleAuthForm, StandardForms.pwdAuthFormId, new WorkflowLinkHttpRequestParameter
        {
            Method = HttpMethods.POST,
            IsAntiforgeryEnabled = true,
            Target = "https://localhost:5001/{realm}/pwd/Authenticate",
            TargetTransformer = new RegexTransformerParameters()
            {
                Rules = new ObservableCollection<MappingRule>
                {
                    new MappingRule { Source = "$.Realm", Target = "realm" }
                }
            }
        })
        .AddLinkHttpRequestAction(StandardForms.ConsoleAuthForm, FormBuilder.Constants.EmptyStep, StandardForms.consoleSendConfirmationCode, new WorkflowLinkHttpRequestParameter
        {
            Method = HttpMethods.POST,
            IsAntiforgeryEnabled = true,
            Target = "https://localhost:5001/{realm}/console/Authenticate",
            TargetTransformer = new RegexTransformerParameters()
            {
                Rules = new ObservableCollection<MappingRule>
                {
                    new MappingRule { Source = "$.Realm", Target = "realm" }
                }
            }
        })
        .AddLinkHttpRequestAction(StandardForms.ConsoleAuthForm, FormBuilder.Constants.EmptyStep, StandardForms.consoleAuthForm, new WorkflowLinkHttpRequestParameter
        {
            Method = HttpMethods.POST,
            IsAntiforgeryEnabled = true,
            Target = "https://localhost:5001/{realm}/console/Authenticate",
            TargetTransformer = new RegexTransformerParameters()
            {
                Rules = new ObservableCollection<MappingRule>
                {
                    new MappingRule { Source = "$.Realm", Target = "realm" }
                }
            }
        })
        .Build();

    public static WorkflowRecord PwdEmailWorkflow = WorkflowBuilder.New(pwdEmailWorkflowId)
        .AddStep(StandardForms.LoginPwdAuthForm, new Coordinate(100, 100))
        .AddStep(StandardForms.EmailAuthForm, new Coordinate(200, 100))
        .AddStep(FormBuilder.Constants.EmptyStep, new Coordinate(300, 100))
        .AddLinkHttpRequestAction(StandardForms.LoginPwdAuthForm, StandardForms.EmailAuthForm, StandardForms.pwdAuthFormId, new WorkflowLinkHttpRequestParameter
        {
            Method = HttpMethods.POST,
            IsAntiforgeryEnabled = true,
            Target = "https://localhost:5001/{realm}/pwd/Authenticate",
            TargetTransformer = new RegexTransformerParameters()
            {
                Rules = new ObservableCollection<MappingRule>
                {
                    new MappingRule { Source = "$.Realm", Target = "realm" }
                }
            }
        })
        .AddLinkHttpRequestAction(StandardForms.EmailAuthForm, FormBuilder.Constants.EmptyStep, StandardForms.emailSendConfirmationCode, new WorkflowLinkHttpRequestParameter
        {
            Method = HttpMethods.POST,
            IsAntiforgeryEnabled = true,
            Target = "https://localhost:5001/{realm}/email/Authenticate",
            TargetTransformer = new RegexTransformerParameters()
            {
                Rules = new ObservableCollection<MappingRule>
                {
                    new MappingRule { Source = "$.Realm", Target = "realm" }
                }
            }
        })
        .AddLinkHttpRequestAction(StandardForms.EmailAuthForm, FormBuilder.Constants.EmptyStep, StandardForms.emailAuthForm, new WorkflowLinkHttpRequestParameter
        {
            Method = HttpMethods.POST,
            IsAntiforgeryEnabled = true,
            Target = "https://localhost:5001/{realm}/email/Authenticate",
            TargetTransformer = new RegexTransformerParameters()
            {
                Rules = new ObservableCollection<MappingRule>
                {
                    new MappingRule { Source = "$.Realm", Target = "realm" }
                }
            }
        })
        .Build();

    public static WorkflowRecord PwdSmsWorkflow = WorkflowBuilder.New(pwdSmsWorkflowId)
        .AddStep(StandardForms.LoginPwdAuthForm, new Coordinate(100, 100))
        .AddStep(StandardForms.SmsAuthForm, new Coordinate(200, 100))
        .AddStep(FormBuilder.Constants.EmptyStep, new Coordinate(300, 100))
        .AddLinkHttpRequestAction(StandardForms.LoginPwdAuthForm, StandardForms.SmsAuthForm, StandardForms.pwdAuthFormId, new WorkflowLinkHttpRequestParameter
        {
            Method = HttpMethods.POST,
            IsAntiforgeryEnabled = true,
            Target = "https://localhost:5001/{realm}/pwd/Authenticate",
            TargetTransformer = new RegexTransformerParameters()
            {
                Rules = new ObservableCollection<MappingRule>
                {
                    new MappingRule { Source = "$.Realm", Target = "realm" }
                }
            }
        })
        .AddLinkHttpRequestAction(StandardForms.SmsAuthForm, FormBuilder.Constants.EmptyStep, StandardForms.smsSendConfirmationCode, new WorkflowLinkHttpRequestParameter
        {
            Method = HttpMethods.POST,
            IsAntiforgeryEnabled = true,
            Target = "https://localhost:5001/{realm}/sms/Authenticate",
            TargetTransformer = new RegexTransformerParameters()
            {
                Rules = new ObservableCollection<MappingRule>
                {
                    new MappingRule { Source = "$.Realm", Target = "realm" }
                }
            }
        })
        .AddLinkHttpRequestAction(StandardForms.SmsAuthForm, FormBuilder.Constants.EmptyStep, StandardForms.smsAuthForm, new WorkflowLinkHttpRequestParameter
        {
            Method = HttpMethods.POST,
            IsAntiforgeryEnabled = true,
            Target = "https://localhost:5001/{realm}/sms/Authenticate",
            TargetTransformer = new RegexTransformerParameters()
            {
                Rules = new ObservableCollection<MappingRule>
                {
                    new MappingRule { Source = "$.Realm", Target = "realm" }
                }
            }
        })
        .Build();

    public static List<WorkflowRecord> AllWorkflows => new List<WorkflowRecord>
    {
        DefaultWorkflow,
        PwdConsoleWorkflow,
        PwdEmailWorkflow,
        PwdSmsWorkflow
    };
}
