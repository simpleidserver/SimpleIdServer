// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;

namespace SimpleIdServer.FastFed.IdentityProvider.Apis.ProviderMetadata;

public class ProviderMetadataController : Controller
{
    public IActionResult Get()
    {
        return new OkObjectResult(new Models.ProviderMetadata());
    }
}
