// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;

namespace SimpleIdServer.IdServer.Api.Jwks
{
    public class JwksController : Controller
    {
        private readonly IJwksRequestHandler _jwksRequestHandler;

        public JwksController(IJwksRequestHandler jwksRequestHandler)
        {
            _jwksRequestHandler = jwksRequestHandler;
        }

        [HttpGet]
        public IActionResult Get([FromRoute] string prefix)
        {
            var jwks = _jwksRequestHandler.Get(prefix);
            return new OkObjectResult(jwks);
        }
    }
}
