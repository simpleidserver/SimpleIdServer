// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder.Link;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Models.Transformer;
using FormBuilder.Transformers;
using SimpleIdServer.IdServer.Console;
using SimpleIdServer.IdServer.Console.UI.ViewModels;
using System.Collections.ObjectModel;

namespace FormBuilder.Builders;

public static class StandardConsoleAuthWorkflows
{
    public static string workflowId = "e7593fa9-5a73-41a3-bfb5-e489fabbe17a";

    public static WorkflowRecord DefaultWorkflow = WorkflowBuilder.New(workflowId, "standardConsoleAuth")
        .AddConsoleAuth()
        .Build(DateTime.UtcNow);

    public static WorkflowBuilder AddConsoleAuth(this WorkflowBuilder builder)
    {
        builder.AddStep(StandardConsoleAuthForms.ConsoleForm)
            .AddLinkHttpRequestAction(StandardConsoleAuthForms.ConsoleForm, Constants.EmptyStep, StandardConsoleAuthForms.consoleSendConfirmationCode, new WorkflowLinkHttpRequestParameter
            {
                Method = HttpMethods.POST,
                IsAntiforgeryEnabled = true,
                Target = "/{realm}/" + SimpleIdServer.IdServer.Console.Constants.AMR + "/Authenticate",
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
            })
            .AddLinkHttpRequestAction(StandardConsoleAuthForms.ConsoleForm, Constants.EmptyStep, StandardConsoleAuthForms.consoleAuthForm, new WorkflowLinkHttpRequestParameter
            {
                Method = HttpMethods.POST,
                IsAntiforgeryEnabled = true,
                Target = "/{realm}/" + SimpleIdServer.IdServer.Console.Constants.AMR + "/Authenticate",
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
            });
        return builder;
    }
}
