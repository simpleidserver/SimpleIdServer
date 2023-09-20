// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System;

namespace SimpleIdServer.IdServer.Builders;

public class RegistrationWorkflowBuilder
{
    private readonly RegistrationWorkflow _registrationWorkflow;

    private RegistrationWorkflowBuilder(RegistrationWorkflow registrationWorkflow)
    {
        _registrationWorkflow = registrationWorkflow;
    }

    public static RegistrationWorkflowBuilder New(string name, bool isDefault = false, string realm = null)
    {
        return new RegistrationWorkflowBuilder(new RegistrationWorkflow { CreateDateTime = DateTime.UtcNow, UpdateDateTime = DateTime.UtcNow, RealmName = realm ?? Constants.DefaultRealm, IsDefault = isDefault, Id = Guid.NewGuid().ToString(), Name = name });
    }

    public RegistrationWorkflowBuilder AddStep(string amr)
    {
        _registrationWorkflow.Steps.Add(amr);
        return this;
    }

    public RegistrationWorkflow Build() => _registrationWorkflow;
}
