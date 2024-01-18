// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Configuration;
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Provisioning;

public abstract class BaseProvisioningService<T> : IProvisioningService
{
    private readonly IConfiguration _configuration;

    public BaseProvisioningService(IConfiguration configuration)
    {
        _configuration = configuration;    
    }

    public abstract string Name { get; }

    public abstract Task<ExtractedResult> ExtractTestData(IdentityProvisioningDefinition definition, CancellationToken cancellationToken);

    public abstract Task<ExtractedResult> Extract(ExtractionPage currentPage, IdentityProvisioningDefinition definition, CancellationToken cancellationToken);

    public abstract Task<List<ExtractionPage>> Paginate(IdentityProvisioningDefinition definition, CancellationToken cancellationToken);

    public T GetOptions(IdentityProvisioningDefinition definition)
    {
        var section = _configuration.GetSection($"{definition.Name}:{typeof(T).Name}");
        return section.Get<T>();
    }
}
