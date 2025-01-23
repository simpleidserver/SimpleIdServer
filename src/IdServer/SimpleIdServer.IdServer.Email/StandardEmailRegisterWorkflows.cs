// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Link;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Models.Transformer;
using FormBuilder.Transformers;
using SimpleIdServer.IdServer.Email;
using SimpleIdServer.IdServer.Email.UI.ViewModels;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Collections.ObjectModel;

namespace FormBuilder.Builders;

public static class StandardEmailRegisterWorkflows
{
    public static string workflowId = "d53b24b4-7a8f-4dd3-8fc9-7a3888ab8d93";

    public static WorkflowRecord DefaultWorkflow = WorkflowBuilder.New(workflowId)
        .AddEmailRegistration()
        .Build();

    public static WorkflowBuilder AddEmailRegistration(this WorkflowBuilder builder, FormRecord? nextStep = null)
    {
        builder.AddStep(StandardEmailRegistrationForms.EmailForm, new Coordinate(100, 100))
            .AddLinkHttpRequestAction(StandardEmailRegistrationForms.EmailForm, Constants.EmptyStep, StandardEmailRegistrationForms.emailSendConfirmationCodeFormId, new WorkflowLinkHttpRequestParameter
            {
                Method = HttpMethods.POST,
                IsAntiforgeryEnabled = true,
                Target = "/{realm}/" + SimpleIdServer.IdServer.Email.Constants.AMR + "/Register",
                Transformers = new List<ITransformerParameters>
                {
                    new RegexTransformerParameters()
                    {
                        Rules = new ObservableCollection<MappingRule>
                        {
                            new MappingRule { Source = $"$.{nameof(RegisterEmailViewModel.Realm)}", Target = "realm" }
                        }
                    },
                    new RelativeUrlTransformerParameters()
                }
            })
            .AddLinkHttpRequestAction(StandardEmailRegistrationForms.EmailForm, nextStep ?? Constants.EmptyStep, StandardEmailRegistrationForms.emailRegisterFormId, new WorkflowLinkHttpRequestParameter
            {
                Method = HttpMethods.POST,
                IsAntiforgeryEnabled = true,
                Target = "/{realm}/" + SimpleIdServer.IdServer.Email.Constants.AMR + "/Register",
                Transformers = new List<ITransformerParameters>
                {
                    new RegexTransformerParameters()
                    {
                        Rules = new ObservableCollection<MappingRule>
                        {
                            new MappingRule { Source = $"$.{nameof(RegisterEmailViewModel.Realm)}", Target = "realm" }
                        }
                    },
                    new RelativeUrlTransformerParameters()
                }
            })
             .AddTransformedLinkUrlAction(StandardEmailRegistrationForms.EmailForm, nextStep ?? Constants.EmptyStep, StandardEmailRegistrationForms.backButtonId, "{returnUrl}", new List<ITransformerParameters>
             {
                 new RegexTransformerParameters
                 {
                     Rules = new ObservableCollection<MappingRule>
                     {
                         new MappingRule { Source = $"$.{nameof(IRegisterViewModel.ReturnUrl)}", Target = "returnUrl" }
                     }
                 }
             });
        return builder;
    }
}
