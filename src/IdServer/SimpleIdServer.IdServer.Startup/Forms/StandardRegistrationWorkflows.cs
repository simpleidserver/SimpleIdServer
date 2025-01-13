// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Builders;
using FormBuilder.Link;
using FormBuilder.Models.Rules;
using FormBuilder.Models;
using FormBuilder.Transformers;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Startup.Forms;

public class StandardRegistrationWorkflows
{
    public static string emailWorkflowId = "d53b24b4-7a8f-4dd3-8fc9-7a3888ab8d93";
    public static string smsWorkflowId = "0ba03d04-2990-4153-8484-0cb8092959cd";
    public static string pwdWorkflowId = "849e51f7-78a8-4a55-9609-88a5b2585870";
    public static string webauthWorkflowId = "af842842-cb22-4011-8c49-aabf09b2c455";
    public static string mobileWorkflowId = "d97459ee-b831-4ca3-9de0-95437c7e7a93";
    public static string vpWorkflowId = "6bccad47-06de-4564-af0b-b63f52ac51aa";

    #region Workflows

    public static WorkflowRecord EmailWorkflow = WorkflowBuilder.New(emailWorkflowId)
        .AddStep(StandardRegistrationForms.EmailForm, new Coordinate(100, 100))
        .AddStep(FormBuilder.Constants.EmptyStep, new Coordinate(200, 100))
        .AddLinkHttpRequestAction(StandardRegistrationForms.EmailForm, FormBuilder.Constants.EmptyStep, StandardRegistrationForms.emailSendConfirmationCodeFormId, new WorkflowLinkHttpRequestParameter
        {
            Method = HttpMethods.POST,
            IsAntiforgeryEnabled = true,
            Target = "https://localhost:5001/{realm}/email/Register",
            TargetTransformer = new RegexTransformerParameters()
            {
                Rules = new ObservableCollection<MappingRule>
                {
                    new MappingRule { Source = "$.Realm", Target = "realm" }
                }
            }
        })
        .AddLinkHttpRequestAction(StandardRegistrationForms.EmailForm, FormBuilder.Constants.EmptyStep, StandardRegistrationForms.emailRegisterFormId, new WorkflowLinkHttpRequestParameter
        {
            Method = HttpMethods.POST,
            IsAntiforgeryEnabled = true,
            Target = "https://localhost:5001/{realm}/email/Register",
            TargetTransformer = new RegexTransformerParameters()
            {
                Rules = new ObservableCollection<MappingRule>
                {
                    new MappingRule { Source = "$.Realm", Target = "realm" }
                }
            }
        })
        .Build();

    public static WorkflowRecord SmsWorkflow = WorkflowBuilder.New(smsWorkflowId)
        .AddStep(StandardRegistrationForms.SmsForm, new Coordinate(100, 100))
        .AddStep(FormBuilder.Constants.EmptyStep, new Coordinate(200, 100))
        .AddLinkHttpRequestAction(StandardRegistrationForms.SmsForm, FormBuilder.Constants.EmptyStep, StandardRegistrationForms.smsSendConfirmationCodeFormId, new WorkflowLinkHttpRequestParameter
        {
            Method = HttpMethods.POST,
            IsAntiforgeryEnabled = true,
            Target = "https://localhost:5001/{realm}/sms/Register",
            TargetTransformer = new RegexTransformerParameters()
            {
                Rules = new ObservableCollection<MappingRule>
                {
                    new MappingRule { Source = "$.Realm", Target = "realm" }
                }
            }
        })
        .AddLinkHttpRequestAction(StandardRegistrationForms.SmsForm, FormBuilder.Constants.EmptyStep, StandardRegistrationForms.smsRegisterFormId, new WorkflowLinkHttpRequestParameter
        {
            Method = HttpMethods.POST,
            IsAntiforgeryEnabled = true,
            Target = "https://localhost:5001/{realm}/sms/Register",
            TargetTransformer = new RegexTransformerParameters()
            {
                Rules = new ObservableCollection<MappingRule>
                {
                    new MappingRule { Source = "$.Realm", Target = "realm" }
                }
            }
        })
        .Build();

    public static WorkflowRecord PwdWorkflow = WorkflowBuilder.New(pwdWorkflowId)
        .AddStep(StandardRegistrationForms.PwdForm, new Coordinate(100, 100))
        .AddStep(FormBuilder.Constants.EmptyStep, new Coordinate(200, 100))
        .AddLinkHttpRequestAction(StandardRegistrationForms.PwdForm, FormBuilder.Constants.EmptyStep, StandardRegistrationForms.pwdRegisterFormId, new WorkflowLinkHttpRequestParameter
        {
            Method = HttpMethods.POST,
            IsAntiforgeryEnabled = true,
            Target = "https://localhost:5001/{realm}/pwd/Register",
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
        .AddStep(StandardRegistrationForms.WebauthnForm, new Coordinate(100, 100))
        .AddStep(FormBuilder.Constants.EmptyStep, new Coordinate(200, 100))
        .AddLinkHttpRequestAction(StandardRegistrationForms.WebauthnForm, FormBuilder.Constants.EmptyStep, StandardRegistrationForms.webauthnFormId, new WorkflowLinkHttpRequestParameter
        {
            Method = HttpMethods.POST,
            IsAntiforgeryEnabled = true,
            Target = "https://localhost:5001/{realm}/webauthn/Register",
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
        .AddStep(StandardRegistrationForms.MobileForm, new Coordinate(100, 100))
        .AddStep(FormBuilder.Constants.EmptyStep, new Coordinate(200, 100))
        .AddLinkHttpRequestAction(StandardRegistrationForms.MobileForm, FormBuilder.Constants.EmptyStep, StandardRegistrationForms.mobileFormId, new WorkflowLinkHttpRequestParameter())
        .Build();

    public static WorkflowRecord VpWorkflow = WorkflowBuilder.New(vpWorkflowId)
        .AddStep(StandardRegistrationForms.VpRegistrationForm, new Coordinate(100, 100))
        .AddStep(StandardRegistrationForms.DisplayQrCodeForm, new Coordinate(200, 100))
        .AddStep(FormBuilder.Constants.EmptyStep, new Coordinate(300, 100))
        .AddLinkAction(StandardRegistrationForms.VpRegistrationForm, StandardRegistrationForms.DisplayQrCodeForm, StandardRegistrationForms.vpRegistrationFormId)
        .AddLinkAction(StandardRegistrationForms.DisplayQrCodeForm, FormBuilder.Constants.EmptyStep, StandardRegistrationForms.displayQrCodeFormId)
        .Build();


    #endregion

    public static List<WorkflowRecord> AllWorkflows = new List<WorkflowRecord>
    {
        EmailWorkflow,
        SmsWorkflow,
        PwdWorkflow,
        WebauthnWorkflow,
        MobileWorkflow,
        VpWorkflow
    };
}
