// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.Extensions.Logging;
using SimpleIdServer.Scim.ExternalEvents;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Provisioning.Provisioner;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Provisioning.Consumers
{
    public class RepresentationAddedEventConsumer : BaseEventConsumer<RepresentationAddedEvent>, IConsumer<RepresentationAddedEvent>
    {
        public RepresentationAddedEventConsumer(
            IEnumerable<IProvisioner> provisioners,
            IProvisioningConfigurationRepository provisioningConfigurationRepository,
            ILogger<BaseEventConsumer<RepresentationAddedEvent>> logger) : base(provisioners, provisioningConfigurationRepository, logger)
        {
        }

        protected override ProvisioningOperations Type => ProvisioningOperations.ADD;
    }
}
