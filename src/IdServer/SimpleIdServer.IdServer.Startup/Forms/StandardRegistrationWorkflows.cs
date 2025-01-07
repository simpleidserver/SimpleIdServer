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

    #endregion

    public static List<WorkflowRecord> AllWorkflows = new List<WorkflowRecord>
    {
        EmailWorkflow
    };
}
