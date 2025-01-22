// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Link;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Models.Transformer;
using FormBuilder.Transformers;
using SimpleIdServer.IdServer.Fido;
using System.Collections.ObjectModel;

namespace FormBuilder.Builders;

public static class StandardFidoRegistrationWorkflows
{
    public static string webauthWorkflowId = "af842842-cb22-4011-8c49-aabf09b2c455";
    public static string mobileWorkflowId = "d97459ee-b831-4ca3-9de0-95437c7e7a93";

    public static WorkflowRecord WebauthnWorkflow = WorkflowBuilder.New(webauthWorkflowId)
        .AddWebauthnRegistration()
        .Build();

    public static WorkflowRecord MobileWorkflow = WorkflowBuilder.New(mobileWorkflowId)
        .AddMobileRegistration()
        .Build();

    public static WorkflowBuilder AddWebauthnRegistration(this WorkflowBuilder builder, FormRecord? nextStep = null)
    {
        builder.AddStep(StandardFidoRegisterForms.WebauthnForm, new Coordinate(100, 100))
                .AddLinkHttpRequestAction(StandardFidoRegisterForms.WebauthnForm, nextStep ?? Constants.EmptyStep, StandardFidoRegisterForms.webauthnFormId, new WorkflowLinkHttpRequestParameter
                {
                    Method = HttpMethods.POST,
                    IsAntiforgeryEnabled = true,
                    Target = "/{realm}/webauthn/Register",
                    Transformers = new List<ITransformerParameters>
                    {
                        new RegexTransformerParameters()
                        {
                            Rules = new ObservableCollection<MappingRule>
                            {
                                new MappingRule { Source = "$.Realm", Target = "realm" }
                            }
                        },
                        new RelativeUrlTransformerParameters()
                    }
                });
        return builder;
    }

    public static WorkflowBuilder AddMobileRegistration(this WorkflowBuilder builder, FormRecord? nextStep = null)
    {
        builder.AddStep(StandardFidoRegisterForms.MobileForm, new Coordinate(100, 100))
            .AddLinkHttpRequestAction(StandardFidoRegisterForms.MobileForm, nextStep ?? Constants.EmptyStep, StandardFidoRegisterForms.mobileFormId, new WorkflowLinkHttpRequestParameter());
        return builder;
    }
}
