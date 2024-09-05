// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;

namespace SimpleIdServer.FastFed.ApplicationProvider.Apis.ProviderMetadata;

public class ProviderMetadataController : Controller
{
    private readonly IGetApplicationProviderMetadataQuery _getApplicationProviderMetadataQuery;

    public ProviderMetadataController(IGetApplicationProviderMetadataQuery getApplicationProviderMetadataQuery)
    {
        _getApplicationProviderMetadataQuery = getApplicationProviderMetadataQuery;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var result = _getApplicationProviderMetadataQuery.Get();
        return new OkObjectResult(result);
    }
}
