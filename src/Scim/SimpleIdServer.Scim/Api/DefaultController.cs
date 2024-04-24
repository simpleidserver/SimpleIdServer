// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Resources;
using static SimpleIdServer.Scim.SCIMConstants;

namespace SimpleIdServer.Scim.Api
{
    // [Route("{*url}", Order = 999)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DefaultController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return this.BuildError(System.Net.HttpStatusCode.NotFound, Global.UnknownEndpoint, ErrorSCIMTypes.Unknown);
        }

        [HttpPut]
        public IActionResult Put()
        {
            return this.BuildError(System.Net.HttpStatusCode.NotFound, Global.UnknownEndpoint, ErrorSCIMTypes.Unknown);
        }

        [HttpPost]
        public IActionResult Post()
        {
            return this.BuildError(System.Net.HttpStatusCode.NotFound, Global.UnknownEndpoint, ErrorSCIMTypes.Unknown);
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            return this.BuildError(System.Net.HttpStatusCode.NotFound, Global.UnknownEndpoint, ErrorSCIMTypes.Unknown);
        }

        [HttpPatch]
        public IActionResult Patch()
        {
            return this.BuildError(System.Net.HttpStatusCode.NotFound, Global.UnknownEndpoint, ErrorSCIMTypes.Unknown);
        }
    }
}
