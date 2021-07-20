// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Idp.Apis.SSO;
using System.Linq;
using System.Net;
using System.Security.Claims;
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
        public Task<IActionResult> LoginGet([FromQuery] SingleSignOnParameter parameter, CancellationToken cancellationToken)
        {
            return InternalLogin(parameter, cancellationToken);
        }

        [HttpPost("Login")]
        public Task<IActionResult> LoginPost([FromBody] SingleSignOnParameter parameter, CancellationToken cancellationToken)
        {
            return InternalLogin(parameter, cancellationToken);
        }

        private async Task<IActionResult> InternalLogin(SingleSignOnParameter parameter, CancellationToken cancellationToken)
        {
            if(User != null && User.Claims.Any())
            {
                var nameId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);
                parameter.UserId = nameId.Value;
            }

            var result = await _singleSignOnHandler.Handle(parameter, cancellationToken);
            if (result.IsValid)
            {
                return new ContentResult
                {
                    Content = result.Response.SerializeToXmlElement().OuterXml,
                    ContentType = "application/xml",
                    StatusCode = (int)HttpStatusCode.OK
                };
            }

            return RedirectToAction("Login", "Authenticate", new
            {
                SAMLRequest = parameter.SAMLRequest,
                RelayState = parameter.RelayState
            }, result.Amr);
        }
    }
}
