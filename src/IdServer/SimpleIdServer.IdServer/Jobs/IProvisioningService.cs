// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Jobs;

public interface IProvisioningService
{
    string Name { get; }
    IAsyncEnumerable<ExtractedResult> Extract(object options, IdentityProvisioningDefinition definition);
    Task<IEnumerable<string>> GetAllowedAttributes(object options);
}
