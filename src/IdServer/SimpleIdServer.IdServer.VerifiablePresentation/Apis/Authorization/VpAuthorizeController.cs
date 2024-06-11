// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace SimpleIdServer.IdServer.VerifiablePresentation.Apis.Authorization;

public class VpAuthorizeController : Controller
{
    [HttpPost]
    public IActionResult Post()
    {
        var request = Request;
        return NoContent();
    }
}
