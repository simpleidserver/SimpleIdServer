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

    public static RegistrationWorkflowBuilder New(string name, string workflowId, bool isDefault = false, string realm = null)
        => new RegistrationWorkflowBuilder(new RegistrationWorkflow { WorkflowId = workflowId, CreateDateTime = DateTime.UtcNow, UpdateDateTime = DateTime.UtcNow, RealmName = realm ?? Constants.DefaultRealm, IsDefault = isDefault, Id = Guid.NewGuid().ToString(), Name = name });

    public RegistrationWorkflow Build() => _registrationWorkflow;
}
