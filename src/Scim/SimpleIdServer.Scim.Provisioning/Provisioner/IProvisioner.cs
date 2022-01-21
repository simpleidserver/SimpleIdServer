// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domains;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Provisioning.Provisioner
{
    public interface IProvisioner
    {
        ProvisioningConfigurationTypes Type { get; }
        Task Seed(ProvisioningOperations operation, string representationId, JObject representation, ProvisioningConfiguration configuration, CancellationToken cancellationToken);
    }
}
