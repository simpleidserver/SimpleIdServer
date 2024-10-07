// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Threading.Tasks;
using System.Threading;
using SimpleIdServer.FastFed.Authentication.Saml;

namespace SimpleIdServer.FastFed.IdentityProvider.Authentication.Saml;

public interface ISamlClientProvisioningService
{
    Task Provision(string clientId, string metadataUrl, SamlEntrepriseMappingsResult mappings, CancellationToken cancellationToken);
}
