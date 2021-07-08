// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.Extensions.Logging;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.ExternalEvents;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Provisioning.Provisioner;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Provisioning.Consumers
{
    public class RepresentationRemovedEventConsumer : BaseEventConsumer<RepresentationRemovedEvent>, IConsumer<RepresentationRemovedEvent>
    {
        public RepresentationRemovedEventConsumer(
            IEnumerable<IProvisioner> provisioners,
            IProvisioningConfigurationRepository provisioningConfigurationRepository,
            ILogger<BaseEventConsumer<RepresentationRemovedEvent>> logger) : base(provisioners, provisioningConfigurationRepository, logger)
        {
        }

        protected override ProvisioningOperations Type => ProvisioningOperations.DELETE;

        protected override Task LaunchWorkflow(ProvisioningConfiguration configuration, ConsumeContext<RepresentationRemovedEvent> context)
        {
            return Task.CompletedTask;
        }
    }
}
