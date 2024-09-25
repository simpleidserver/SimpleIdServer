// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.FastFed.Models;
using System.Threading.Tasks;
using System.Threading;

namespace SimpleIdServer.FastFed;

public interface IIdProviderProvisioningService
{
    string Name { get; }
    string Area { get; }
    Task<MigrationResult> Migrate(ProvisioningProfileHistory provisioningProfileHistory, CapabilitySettings settings, CancellationToken cancellationToken);
}
