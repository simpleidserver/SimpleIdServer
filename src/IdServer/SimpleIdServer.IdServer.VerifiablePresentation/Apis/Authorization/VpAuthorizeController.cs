// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.VerifiablePresentation.DTOs;

namespace SimpleIdServer.IdServer.VerifiablePresentation.Apis.Authorization;

public class VpAuthorizeController : Controller
{
    [HttpPost]
    public IActionResult Post([FromQuery] IdTokenResponse idTokenResponse)
    {

        var request = Request;
        return NoContent();
    }

    private async Task Validate(IdTokenResponse idTokenResponse)
    {
        if (idTokenResponse == null) throw new OAuthException(null, null);
        if(string.IsNullOrWhiteSpace(idTokenResponse.IdToken)) throw new OAuthException(null, null);

    }
}
