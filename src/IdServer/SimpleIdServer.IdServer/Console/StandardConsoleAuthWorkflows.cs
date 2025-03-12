// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder.Builders;
using FormBuilder.Link;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Models.Transformer;
using FormBuilder.Transformers;
using SimpleIdServer.IdServer.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SimpleIdServer.IdServer.Console;

public static class StandardConsoleAuthWorkflows
{
    public static string workflowId = "e7593fa9-5a73-41a3-bfb5-e489fabbe17a";

    public static WorkflowRecord DefaultWorkflow = WorkflowBuilder.New(workflowId)
        .AddConsoleAuth()
        .Build(DateTime.UtcNow);

    public static WorkflowBuilder AddConsoleAuth(this WorkflowBuilder builder)
    {
        builder.AddStep(StandardConsoleAuthForms.ConsoleForm)
            .AddLinkHttpRequestAction(StandardConsoleAuthForms.ConsoleForm, FormBuilder.Constants.EmptyStep, StandardConsoleAuthForms.consoleSendConfirmationCode, "Confirmation code", new WorkflowLinkHttpRequestParameter
            {
                Method = HttpMethods.POST,
                IsAntiforgeryEnabled = true,
                Target = "/{realm}/" + SimpleIdServer.IdServer.Constants.ConsoleAmr + "/Authenticate",
                Transformers = new List<ITransformerParameters>
                {
                    new RegexTransformerParameters()
                    {
                        Rules = new ObservableCollection<MappingRule>
                        {
                            new MappingRule { Source = $"$.{nameof(AuthenticateConsoleViewModel.Realm)}", Target = "realm" }
                        }
                    },
                    new RelativeUrlTransformerParameters()
                }
            }, false)
            .AddLinkHttpRequestAction(StandardConsoleAuthForms.ConsoleForm, FormBuilder.Constants.EmptyStep, StandardConsoleAuthForms.consoleAuthForm, "Authenticate", new WorkflowLinkHttpRequestParameter
            {
                Method = HttpMethods.POST,
                IsAntiforgeryEnabled = true,
                Target = "/{realm}/" + SimpleIdServer.IdServer.Constants.ConsoleAmr + "/Authenticate",
                Transformers = new List<ITransformerParameters>
                {
                    new RegexTransformerParameters()
                    {
                        Rules = new ObservableCollection<MappingRule>
                        {
                            new MappingRule { Source = $"$.{nameof(AuthenticateConsoleViewModel.Realm)}", Target = "realm" }
                        }
                    },
                    new RelativeUrlTransformerParameters()
                }
            }, true);
        return builder;
    }
}
