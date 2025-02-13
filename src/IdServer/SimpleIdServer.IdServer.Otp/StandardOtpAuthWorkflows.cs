// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Link;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Models.Transformer;
using FormBuilder.Transformers;
using SimpleIdServer.IdServer.Otp;
using SimpleIdServer.IdServer.Otp.UI.ViewModels;
using System.Collections.ObjectModel;

namespace FormBuilder.Builders;

public static class StandardOtpAuthWorkflows
{
    public static string workflowId = "cd3a77fe-4462-4896-8d3c-4d0f77e1942b";

    public static WorkflowRecord DefaultWorkflow = WorkflowBuilder.New(workflowId)
        .AddOtpAuth()
        .Build(DateTime.UtcNow);

    public static WorkflowBuilder AddOtpAuth(this WorkflowBuilder builder)
    {
        builder.AddStep(StandardOtpAuthForms.OtpForm)
            .AddLinkHttpRequestAction(StandardOtpAuthForms.OtpForm, Constants.EmptyStep, StandardOtpAuthForms.otpCodeFormId, "Authenticate", new WorkflowLinkHttpRequestParameter
            {
                Method = HttpMethods.POST,
                IsAntiforgeryEnabled = true,
                Target = "/{realm}/" + SimpleIdServer.IdServer.Otp.Constants.Amr + "/Authenticate",
                Transformers = new List<ITransformerParameters>
                {
                    new RegexTransformerParameters()
                    {
                        Rules = new ObservableCollection<MappingRule>
                        {
                            new MappingRule { Source = $"$.{nameof(AuthenticateOtpViewModel.Realm)}", Target = "realm" }
                        }
                    },
                    new RelativeUrlTransformerParameters()
                }
            });
        return builder;
    }
}