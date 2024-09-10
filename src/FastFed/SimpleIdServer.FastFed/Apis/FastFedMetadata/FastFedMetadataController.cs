// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;

namespace SimpleIdServer.FastFed.Apis.FastFedMetadata;

public class FastFedMetadataController : Controller
{
    private readonly IGetProviderMetadataQuery _getProviderMetadataQuery;

    public FastFedMetadataController(IGetProviderMetadataQuery getProviderMetadataQuery)
    {
        _getProviderMetadataQuery = getProviderMetadataQuery;
    }


    [HttpGet]
    public IActionResult Get()
    {
        var result = _getProviderMetadataQuery.Get();
        return Ok(result);
    }
}