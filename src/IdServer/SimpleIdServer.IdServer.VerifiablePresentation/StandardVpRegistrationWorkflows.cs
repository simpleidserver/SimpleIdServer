// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Models;
using SimpleIdServer.IdServer.VerifiablePresentation;

namespace FormBuilder.Builders;

public static class StandardVpRegistrationWorkflows
{
    public static string workflowId = "6bccad47-06de-4564-af0b-b63f52ac51aa";

    public static WorkflowRecord DefaultWorkflow = WorkflowBuilder.New(workflowId)
        .AddStep(Constants.EmptyStep, new Coordinate(100, 100))
        .AddVpRegistration()
        .Build();

    public static WorkflowBuilder AddVpRegistration(this WorkflowBuilder builder)
    {
        builder.AddStep(StandardVpRegisterForms.VpForm, new Coordinate(100, 100))
        .AddLinkAction(StandardVpRegisterForms.VpForm, FormBuilder.Constants.EmptyStep, StandardVpRegisterForms.vpRegistrationFormId);
        return builder;
    }
}