// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Link;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Models.Transformer;
using FormBuilder.Transformers;
using SimpleIdServer.IdServer.Pwd;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Collections.ObjectModel;

namespace FormBuilder.Builders;

public static class StandardPwdRegistrationWorkflows
{
    public const string workflowId = "849e51f7-78a8-4a55-9609-88a5b2585870";

    public static WorkflowRecord DefaultWorkflow = WorkflowBuilder.New(workflowId)
        .AddPwdRegistration()
        .Build(DateTime.UtcNow);

    public static WorkflowBuilder AddPwdRegistration(this WorkflowBuilder builder, FormRecord? nextStep = null)
    {
        builder.AddStep(StandardPwdRegisterForms.PwdForm)
            .AddLinkHttpRequestAction(StandardPwdRegisterForms.PwdForm, nextStep ?? Constants.EmptyStep, StandardPwdRegisterForms.pwdRegisterFormId, "Register", new WorkflowLinkHttpRequestParameter
            {
                Method = HttpMethods.POST,
                IsAntiforgeryEnabled = true,
                Target = "/{realm}/pwd/Register",
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
            }, true)
            .AddTransformedLinkUrlAction(StandardPwdRegisterForms.PwdForm, nextStep ?? Constants.EmptyStep, StandardPwdRegisterForms.backBtnId, "Back", "{returnUrl}", new List<ITransformerParameters>
            {
                new RegexTransformerParameters
                {
                    Rules = new ObservableCollection<MappingRule>
                    {
                        new MappingRule { Source = $"$.{nameof(IRegisterViewModel.ReturnUrl)}", Target = "returnUrl" }
                    }
                }
            }, false);
        return builder;
    }
}
