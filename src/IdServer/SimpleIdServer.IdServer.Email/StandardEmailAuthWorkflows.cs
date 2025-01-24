// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Link;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Models.Transformer;
using FormBuilder.Transformers;
using SimpleIdServer.IdServer.Email;
using SimpleIdServer.IdServer.Email.UI.ViewModels;
using System.Collections.ObjectModel;

namespace FormBuilder.Builders;

public static class StandardEmailAuthWorkflows
{
    public static string workflowId = "62cb8fcc-34b6-4af9-8d54-db1c98827a08";

    public static WorkflowRecord DefaultWorkflow = WorkflowBuilder.New(workflowId, "standardEmailAuth")
        .AddEmailAuth()
        .Build(DateTime.UtcNow);

    public static WorkflowBuilder AddEmailAuth(this WorkflowBuilder builder)
    {
        builder.AddStep(StandardEmailAuthForms.EmailForm, new Coordinate(100, 100))
            .AddLinkHttpRequestAction(StandardEmailAuthForms.EmailForm, Constants.EmptyStep, StandardEmailAuthForms.emailSendConfirmationCode, new WorkflowLinkHttpRequestParameter
            {
                Method = HttpMethods.POST,
                IsAntiforgeryEnabled = true,
                Target = "/{realm}/" + SimpleIdServer.IdServer.Email.Constants.AMR + "/Authenticate",
                Transformers = new List<ITransformerParameters>
                {
                    new RegexTransformerParameters()
                    {
                        Rules = new ObservableCollection<MappingRule>
                        {
                            new MappingRule { Source = $"$.{nameof(AuthenticateEmailViewModel.Realm)}", Target = "realm" }
                        }
                    },
                    new RelativeUrlTransformerParameters()
                }
            })
            .AddLinkHttpRequestAction(StandardEmailAuthForms.EmailForm, Constants.EmptyStep, StandardEmailAuthForms.emailAuthForm, new WorkflowLinkHttpRequestParameter
            {
                Method = HttpMethods.POST,
                IsAntiforgeryEnabled = true,
                Target = "/{realm}/" + SimpleIdServer.IdServer.Email.Constants.AMR + "/Authenticate",
                Transformers = new List<ITransformerParameters>
                {
                    new RegexTransformerParameters()
                    {
                        Rules = new ObservableCollection<MappingRule>
                        {
                            new MappingRule { Source = $"$.{nameof(AuthenticateEmailViewModel.Realm)}", Target = "realm" }
                        }
                    },
                    new RelativeUrlTransformerParameters()
                }
            });
        return builder;
    }
}
