// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder.Link;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Models.Transformer;
using FormBuilder.Transformers;
using SimpleIdServer.IdServer.Sms;
using SimpleIdServer.IdServer.Sms.UI.ViewModels;
using System.Collections.ObjectModel;

namespace FormBuilder.Builders;

public static class StandardSmsAuthWorkflows
{
    public static string workflowId = "08bea90f-2183-4c56-977f-fd0a9c5e32b8";

    public static WorkflowRecord DefaultWorkflow = WorkflowBuilder.New(workflowId)
        .AddSmsAuth()
        .Build(DateTime.UtcNow);

    public static WorkflowBuilder AddSmsAuth(this WorkflowBuilder builder)
    {
        builder.AddStep(StandardSmsAuthForms.SmsForm)
            .AddLinkHttpRequestAction(StandardSmsAuthForms.SmsForm, Constants.EmptyStep, StandardSmsAuthForms.smsSendConfirmationCode, "Confirmation code", new WorkflowLinkHttpRequestParameter
            {
                Method = HttpMethods.POST,
                IsAntiforgeryEnabled = true,
                Target = "/{realm}/" + SimpleIdServer.IdServer.Sms.Constants.AMR + "/Authenticate",
                Transformers = new List<ITransformerParameters>
                {
                    new RegexTransformerParameters()
                    {
                        Rules = new ObservableCollection<MappingRule>
                        {
                            new MappingRule { Source = $"$.{nameof(AuthenticateSmsViewModel.Realm)}", Target = "realm" }
                        }
                    },
                    new RelativeUrlTransformerParameters()
                }
            })
            .AddLinkHttpRequestAction(StandardSmsAuthForms.SmsForm, Constants.EmptyStep, StandardSmsAuthForms.smsAuthForm, "Authenticate", new WorkflowLinkHttpRequestParameter
            {
                Method = HttpMethods.POST,
                IsAntiforgeryEnabled = true,
                Target = "/{realm}/" + SimpleIdServer.IdServer.Sms.Constants.AMR + "/Authenticate",
                Transformers = new List<ITransformerParameters>
                {
                    new RegexTransformerParameters()
                    {
                        Rules = new ObservableCollection<MappingRule>
                        {
                            new MappingRule { Source = $"$.{nameof(AuthenticateSmsViewModel.Realm)}", Target = "realm" }
                        }
                    },
                    new RelativeUrlTransformerParameters()
                }
            });
        return builder;
    }
}
