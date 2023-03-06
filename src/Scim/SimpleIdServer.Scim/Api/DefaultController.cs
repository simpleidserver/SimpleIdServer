// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Resources;

namespace SimpleIdServer.Scim.Api
{
    [Route("{*url}", Order = 999)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DefaultController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return this.BuildError(System.Net.HttpStatusCode.NotFound, Global.UnknownEndpoint);
        }

        [HttpPut]
        public IActionResult Put()
        {
            return this.BuildError(System.Net.HttpStatusCode.NotFound, Global.UnknownEndpoint);
        }

        [HttpPost]
        public IActionResult Post()
        {
            return this.BuildError(System.Net.HttpStatusCode.NotFound, Global.UnknownEndpoint);
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            return this.BuildError(System.Net.HttpStatusCode.NotFound, Global.UnknownEndpoint);
        }
    }
}
