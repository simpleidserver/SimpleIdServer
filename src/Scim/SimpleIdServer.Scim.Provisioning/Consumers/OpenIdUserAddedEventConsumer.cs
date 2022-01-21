// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.Extensions.Logging;
using SimpleIdServer.OpenID.ExternalEvents;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Provisioning.Provisioner;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Provisioning.Consumers
{
    public class OpenIdUserAddedEventConsumer : BaseEventConsumer<UserAddedEvent>, IConsumer<UserAddedEvent>
    {
        public OpenIdUserAddedEventConsumer(
            IEnumerable<IProvisioner> provisioners,
            IProvisioningConfigurationRepository provisioningConfigurationRepository,
            ILogger<BaseEventConsumer<UserAddedEvent>> logger) : base(provisioners, provisioningConfigurationRepository, logger)
        {
        }

        protected override ProvisioningOperations Type => ProvisioningOperations.ADD;

        protected override Task<WorkflowResult> LaunchWorkflow(ProvisioningConfiguration configuration, ConsumeContext<UserAddedEvent> context)
        {
            return Task.FromResult((WorkflowResult)new WorkflowResult(string.Empty, string.Empty));
        }
    }
}
