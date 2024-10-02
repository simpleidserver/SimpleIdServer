// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.FastFed.Models;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed;

public interface IAppProviderProvisioningService
{
    string Name {  get; }
    string RegisterConfigurationName { get; }
    Task<JsonObject> EnableCapability(IdentityProviderFederation idProviderFederation, JsonWebToken jwt, CancellationToken cancellationToken);
}
