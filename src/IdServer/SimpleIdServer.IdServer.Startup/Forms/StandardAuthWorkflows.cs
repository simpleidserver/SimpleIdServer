// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Builders;
using FormBuilder.Link;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Transformers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SimpleIdServer.IdServer.Startup.Forms;

public class StandardAuthWorkflows
{
    public static string defaultWorkflowId = "241a7509-4c58-4f49-b1df-49011b2c9bcb";
    public static string pwdConsoleWorkflowId = "e7593fa9-5a73-41a3-bfb5-e489fabbe17a";
    public static string pwdEmailWorkflowId = "62cb8fcc-34b6-4af9-8d54-db1c98827a08";
    public static string pwdSmsWorkflowId = "08bea90f-2183-4c56-977f-fd0a9c5e32b8";
    public static string pwdWebauthnWorkflowId = "ad636448-90db-41da-91c7-4e96b981b354";
    public static string webauthWorkflowId = "a725b543-1403-4aab-8329-25b89f07cb48";
    public static string mobileWorkflowId = "1f0a3398-aeb2-42c8-b6e6-ea03396f1a87";
    public static string otpWorkflowId = "cd3a77fe-4462-4896-8d3c-4d0f77e1942b";
    public static string confirmResetPwdWorkflowId = "e05d75d5-5df1-42d4-8c1e-884fc9a2ecff"; 

    public static WorkflowRecord DefaultWorkflow = WorkflowBuilder.New(defaultWorkflowId)
        .AddStep(StandardAuthForms.LoginPwdAuthForm, new Coordinate(100, 100))
        .AddStep(StandardAuthForms.ResetPwdForm, new Coordinate(200, 100))
        .AddStep(FormBuilder.Constants.EmptyStep, new Coordinate(400, 100))
        .AddLinkHttpRequestAction(StandardAuthForms.LoginPwdAuthForm, FormBuilder.Constants.EmptyStep, StandardAuthForms.pwdAuthFormId, new WorkflowLinkHttpRequestParameter
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
        .AddLinkHttpRequestAction(StandardAuthForms.LoginPwdAuthForm, StandardAuthForms.ResetPwdForm, StandardAuthForms.pwdForgetBtnId, new WorkflowLinkHttpRequestParameter
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
        .AddLinkHttpRequestAction(StandardAuthForms.ResetPwdForm, FormBuilder.Constants.EmptyStep, StandardAuthForms.pwdResetFormId, new WorkflowLinkHttpRequestParameter
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
        .AddStep(StandardAuthForms.LoginPwdAuthForm, new Coordinate(100, 100))
        .AddStep(StandardAuthForms.ConsoleAuthForm, new Coordinate(200, 100))
        .AddStep(FormBuilder.Constants.EmptyStep, new Coordinate(300, 100))
        .AddLinkHttpRequestAction(StandardAuthForms.LoginPwdAuthForm, StandardAuthForms.ConsoleAuthForm, StandardAuthForms.pwdAuthFormId, new WorkflowLinkHttpRequestParameter
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
        .AddLinkHttpRequestAction(StandardAuthForms.ConsoleAuthForm, FormBuilder.Constants.EmptyStep, StandardAuthForms.consoleSendConfirmationCode, new WorkflowLinkHttpRequestParameter
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
        .AddLinkHttpRequestAction(StandardAuthForms.ConsoleAuthForm, FormBuilder.Constants.EmptyStep, StandardAuthForms.consoleAuthForm, new WorkflowLinkHttpRequestParameter
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
        .AddStep(StandardAuthForms.LoginPwdAuthForm, new Coordinate(100, 100))
        .AddStep(StandardAuthForms.EmailAuthForm, new Coordinate(200, 100))
        .AddStep(FormBuilder.Constants.EmptyStep, new Coordinate(300, 100))
        .AddLinkHttpRequestAction(StandardAuthForms.LoginPwdAuthForm, StandardAuthForms.EmailAuthForm, StandardAuthForms.pwdAuthFormId, new WorkflowLinkHttpRequestParameter
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
        .AddLinkHttpRequestAction(StandardAuthForms.EmailAuthForm, FormBuilder.Constants.EmptyStep, StandardAuthForms.emailSendConfirmationCode, new WorkflowLinkHttpRequestParameter
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
        .AddLinkHttpRequestAction(StandardAuthForms.EmailAuthForm, FormBuilder.Constants.EmptyStep, StandardAuthForms.emailAuthForm, new WorkflowLinkHttpRequestParameter
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
        .AddStep(StandardAuthForms.LoginPwdAuthForm, new Coordinate(100, 100))
        .AddStep(StandardAuthForms.SmsAuthForm, new Coordinate(200, 100))
        .AddStep(FormBuilder.Constants.EmptyStep, new Coordinate(300, 100))
        .AddLinkHttpRequestAction(StandardAuthForms.LoginPwdAuthForm, StandardAuthForms.SmsAuthForm, StandardAuthForms.pwdAuthFormId, new WorkflowLinkHttpRequestParameter
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
        .AddLinkHttpRequestAction(StandardAuthForms.SmsAuthForm, FormBuilder.Constants.EmptyStep, StandardAuthForms.smsSendConfirmationCode, new WorkflowLinkHttpRequestParameter
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
        .AddLinkHttpRequestAction(StandardAuthForms.SmsAuthForm, FormBuilder.Constants.EmptyStep, StandardAuthForms.smsAuthForm, new WorkflowLinkHttpRequestParameter
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

    public static WorkflowRecord PwdWebauthnWorkflow = WorkflowBuilder.New(pwdWebauthnWorkflowId)
        .AddStep(StandardAuthForms.LoginPwdAuthForm, new Coordinate(100, 100))
        .AddStep(StandardAuthForms.WebauthnForm, new Coordinate(200, 100))
        .AddStep(FormBuilder.Constants.EmptyStep, new Coordinate(300, 100))
        .AddLinkHttpRequestAction(StandardAuthForms.LoginPwdAuthForm, StandardAuthForms.WebauthnForm, StandardAuthForms.pwdAuthFormId, new WorkflowLinkHttpRequestParameter
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
        .AddLinkHttpRequestAction(StandardAuthForms.WebauthnForm, FormBuilder.Constants.EmptyStep, StandardAuthForms.webauthnFormId, new WorkflowLinkHttpRequestParameter
        {
            Method = HttpMethods.POST,
            IsAntiforgeryEnabled = true,
            Target = "https://localhost:5001/{realm}/webauthn/Authenticate",
            TargetTransformer = new RegexTransformerParameters()
            {
                Rules = new ObservableCollection<MappingRule>
                {
                    new MappingRule { Source = "$.Realm", Target = "realm" }
                }
            }
        })
        .Build();

    public static WorkflowRecord WebauthnWorkflow = WorkflowBuilder.New(webauthWorkflowId)
        .AddStep(StandardAuthForms.WebauthnForm, new Coordinate(100, 100))
        .AddStep(FormBuilder.Constants.EmptyStep, new Coordinate(200, 100))
        .AddLinkHttpRequestAction(StandardAuthForms.WebauthnForm, FormBuilder.Constants.EmptyStep, StandardAuthForms.webauthnFormId, new WorkflowLinkHttpRequestParameter
        {
            Method = HttpMethods.POST,
            IsAntiforgeryEnabled = true,
            Target = "https://localhost:5001/{realm}/webauthn/Authenticate",
            TargetTransformer = new RegexTransformerParameters()
            {
                Rules = new ObservableCollection<MappingRule>
                {
                    new MappingRule { Source = "$.Realm", Target = "realm" }
                }
            }
        })
        .Build();

    public static WorkflowRecord MobileWorkflow = WorkflowBuilder.New(mobileWorkflowId)
        .AddStep(StandardAuthForms.MobileForm, new Coordinate(100, 100))
        .AddStep(StandardAuthForms.DisplayQrCodeForm, new Coordinate(200, 100))
        .AddStep(FormBuilder.Constants.EmptyStep, new Coordinate(200, 100))
        .AddLinkAction(StandardAuthForms.MobileForm, StandardAuthForms.DisplayQrCodeForm, StandardAuthForms.mobileFormId)
        .AddLinkAction(StandardAuthForms.DisplayQrCodeForm, FormBuilder.Constants.EmptyStep, StandardAuthForms.displayQrCodeFormId)
        .Build();

    public static WorkflowRecord OtpWorkflow = WorkflowBuilder.New(otpWorkflowId)
        .AddStep(StandardAuthForms.OtpForm, new Coordinate(100, 100))
        .AddStep(FormBuilder.Constants.EmptyStep, new Coordinate(200, 100))
        .AddLinkHttpRequestAction(StandardAuthForms.OtpForm, FormBuilder.Constants.EmptyStep, StandardAuthForms.otpCodeFormId, new WorkflowLinkHttpRequestParameter
        {
            Method = HttpMethods.POST,
            IsAntiforgeryEnabled = true,
            Target = "https://localhost:5001/{realm}/otp/Authenticate",
            TargetTransformer = new RegexTransformerParameters()
            {
                Rules = new ObservableCollection<MappingRule>
                {
                    new MappingRule { Source = "$.Realm", Target = "realm" }
                }
            }
        })
        .Build();

    public static WorkflowRecord ConfirmResetPwdWorkflow = WorkflowBuilder.New(confirmResetPwdWorkflowId)
        .AddStep(StandardAuthForms.ConfirmResetPwdForm, new Coordinate(100, 100))
        .AddStep(FormBuilder.Constants.EmptyStep, new Coordinate(200, 100))
        .AddLinkHttpRequestAction(StandardAuthForms.ConfirmResetPwdForm, FormBuilder.Constants.EmptyStep, StandardAuthForms.confirmResetPwdFormId, new WorkflowLinkHttpRequestParameter
        {
            Method = HttpMethods.POST,
            IsAntiforgeryEnabled = true,
            Target = "https://localhost:5001/{realm}/pwd/Reset/Confirm",
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
        PwdSmsWorkflow,
        PwdWebauthnWorkflow,
        WebauthnWorkflow,
        MobileWorkflow,
        OtpWorkflow,
        ConfirmResetPwdWorkflow
    };
}
