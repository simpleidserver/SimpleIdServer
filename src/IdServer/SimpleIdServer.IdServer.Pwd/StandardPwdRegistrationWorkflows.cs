// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Link;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Models.Transformer;
using FormBuilder.Transformers;
using SimpleIdServer.IdServer.Pwd;
using System.Collections.ObjectModel;

namespace FormBuilder.Builders;

public static class StandardPwdRegistrationWorkflows
{
    public const string workflowId = "849e51f7-78a8-4a55-9609-88a5b2585870";

    public static WorkflowRecord DefaultWorkflow = WorkflowBuilder.New(workflowId)
        .AddStep(Constants.EmptyStep, new Coordinate(100, 100))
        .AddPwdRegistration()
        .Build();

    public static WorkflowBuilder AddPwdRegistration(this WorkflowBuilder builder)
    {
        builder.AddStep(StandardPwdRegisterForms.PwdForm, new Coordinate(100, 100))
            .AddLinkHttpRequestAction(StandardPwdRegisterForms.PwdForm, Constants.EmptyStep, StandardPwdRegisterForms.pwdRegisterFormId, new WorkflowLinkHttpRequestParameter
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
            });
        return builder;
    }
}
