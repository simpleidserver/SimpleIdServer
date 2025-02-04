// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Link;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Models.Transformer;
using FormBuilder.Transformers;
using SimpleIdServer.IdServer.Fido;
using SimpleIdServer.IdServer.Fido.UI.ViewModels;
using System.Collections.ObjectModel;

namespace FormBuilder.Builders;

public static class StandardFidoAuthWorkflows
{
    public static string webauthnWorkflowId = "a725b543-1403-4aab-8329-25b89f07cb48";
    public static string mobileWorkflowId = "1f0a3398-aeb2-42c8-b6e6-ea03396f1a87";

    public static WorkflowRecord DefaultWebauthnWorkflow = WorkflowBuilder.New(webauthnWorkflowId, "defaultWebauthnAuth")
        .AddWebauthnAuth()
        .Build(DateTime.UtcNow);

    public static WorkflowRecord DefaultMobileWorkflow = WorkflowBuilder.New(mobileWorkflowId, "defaultMobileAuth")
        .AddMobileAuth()
        .Build(DateTime.UtcNow);

    public static WorkflowBuilder AddWebauthnAuth(this WorkflowBuilder builder)
    {
        builder.AddStep(StandardFidoAuthForms.WebauthnForm)
            .AddLinkHttpRequestAction(StandardFidoAuthForms.WebauthnForm, FormBuilder.Constants.EmptyStep, StandardFidoAuthForms.webauthnFormId, new WorkflowLinkHttpRequestParameter
            {
                Method = HttpMethods.POST,
                IsAntiforgeryEnabled = true,
                Target = "/{realm}/"+ SimpleIdServer.IdServer.Fido.Constants.AMR +"/Authenticate",
                Transformers = new List<ITransformerParameters>
                {
                    new RegexTransformerParameters()
                    {
                        Rules = new ObservableCollection<MappingRule>
                        {
                            new MappingRule { Source = $"$.{nameof(AuthenticateWebauthnViewModel.Realm)}", Target = "realm" }
                        }
                    },
                    new RelativeUrlTransformerParameters()

                }
            });
        return builder;
    }

    public static WorkflowBuilder AddMobileAuth(this WorkflowBuilder builder)
    {
        builder.AddStep(StandardFidoAuthForms.MobileForm)
            .AddLinkHttpRequestAction(StandardFidoAuthForms.MobileForm, FormBuilder.Constants.EmptyStep, StandardFidoAuthForms.mobileFormId, new WorkflowLinkHttpRequestParameter
            {
                Method = HttpMethods.POST,
                IsAntiforgeryEnabled = true,
                Target = "/{realm}/" + SimpleIdServer.IdServer.Fido.Constants.MobileAMR + "/Authenticate",
                Transformers = new List<ITransformerParameters>
                {
                    new RegexTransformerParameters()
                    {
                        Rules = new ObservableCollection<MappingRule>
                        {
                            new MappingRule { Source = $"$.{nameof(AuthenticateWebauthnViewModel.Realm)}", Target = "realm" }
                        }
                    },
                    new RelativeUrlTransformerParameters()

                }
            });
        return builder;
    }
}
