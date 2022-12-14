// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;

namespace SimpleIdServer.OAuth.Api.Jwks
{
    [Route(Constants.EndPoints.Jwks)]
    public class JwksController : Controller
    {
        private readonly IJwksRequestHandler _jwksRequestHandler;

        public JwksController(IJwksRequestHandler jwksRequestHandler)
        {
            _jwksRequestHandler = jwksRequestHandler;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var jwks = _jwksRequestHandler.Get();
            return new OkObjectResult(jwks);
        }
    }
}
