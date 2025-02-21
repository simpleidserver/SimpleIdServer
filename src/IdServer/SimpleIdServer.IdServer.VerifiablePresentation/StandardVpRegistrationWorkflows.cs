// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Models.Transformer;
using FormBuilder.Transformers;
using SimpleIdServer.IdServer.UI.ViewModels;
using SimpleIdServer.IdServer.VerifiablePresentation;
using System.Collections.ObjectModel;

namespace FormBuilder.Builders;

public static class StandardVpRegistrationWorkflows
{
    public static string workflowId = "6bccad47-06de-4564-af0b-b63f52ac51aa";

    public static WorkflowRecord DefaultWorkflow = WorkflowBuilder.New(workflowId)
        .AddVpRegistration()
        .Build(DateTime.UtcNow);

    public static WorkflowBuilder AddVpRegistration(this WorkflowBuilder builder, FormRecord? nextStep = null)
    {
        builder.AddStep(StandardVpRegisterForms.VpForm)
            .AddLinkAction(StandardVpRegisterForms.VpForm, nextStep ?? Constants.EmptyStep, StandardVpRegisterForms.vpRegistrationFormId, "Choose VP", false)
            .AddTransformedLinkUrlAction(StandardVpRegisterForms.VpForm, nextStep ?? Constants.EmptyStep, StandardVpRegisterForms.backBtnId, "Register", "{returnUrl}", new List<ITransformerParameters>
            {
                new RegexTransformerParameters
                {
                    Rules = new ObservableCollection<MappingRule>
                    {
                        new MappingRule { Source = $"$.{nameof(IRegisterViewModel.ReturnUrl)}", Target = "returnUrl" }
                    }
                }
            }, true);
        return builder;
    }
}