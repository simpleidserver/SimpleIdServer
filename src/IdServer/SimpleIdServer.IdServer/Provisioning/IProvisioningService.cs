// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Provisioning;

public interface IProvisioningService
{
    string Name { get; }
    Task<ExtractedResult> ExtractTestData(IdentityProvisioningDefinition definition, CancellationToken cancellationToken);
    Task<ExtractedResult> Extract(ExtractionPage currentPage, IdentityProvisioningDefinition definition, CancellationToken cancellationToken);
    Task<List<ExtractionPage>> Paginate(IdentityProvisioningDefinition definition, CancellationToken cancellationToken);
}

public class ExtractionPage
{
    public int BatchSize { get; set; }
    public int Page { get; set; }
    public byte[] Cookie { get; set; }
}