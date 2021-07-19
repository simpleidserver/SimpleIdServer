// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.Saml.Idp.Apis.SSO;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Idp.SSO.Apis
{
    [Route(Constants.RouteNames.SingleSignOn)]
    public class SingleSignOnController : Controller
    {
        private readonly ISingleSignOnHandler _singleSignOnHandler;

        public SingleSignOnController(ISingleSignOnHandler singleSignOnHandler)
        {
            _singleSignOnHandler = singleSignOnHandler;
        }

        [HttpGet("Login")]
        public async Task<IActionResult> LoginGet([FromQuery] SingleSignOnParameter parameter, CancellationToken cancellationToken)
        {
            await _singleSignOnHandler.Handle(parameter, cancellationToken);
            return null;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginPost([FromBody] SingleSignOnParameter parameter, CancellationToken cancellationToken)
        {
            await _singleSignOnHandler.Handle(parameter, cancellationToken);
            return null;
        }
    }
}
