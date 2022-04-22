// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.Saml.DTOs;
using SimpleIdServer.Saml.Idp.Apis.SSO;
using System.Linq;
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
        public Task<IActionResult> LoginGet([FromQuery] SAMLRequestDto parameter, CancellationToken cancellationToken)
        {
            return InternalLogin(parameter, cancellationToken);
        }

        [HttpPost("Login")]
        public Task<IActionResult> LoginPostBody(SAMLRequestDto parameter, CancellationToken cancellationToken)
        {
            return InternalLogin(parameter, cancellationToken);
        }

        private async Task<IActionResult> InternalLogin(SAMLRequestDto parameter, CancellationToken cancellationToken)
        {
            var nameId = string.Empty;
            if (User != null && User.Claims.Any())
            {
                nameId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            }

            var result = await _singleSignOnHandler.Handle(parameter, nameId, cancellationToken);
            switch(result.Action)
            {
                case SingleSignOnActions.AUTHENTICATE:
                    return RedirectToAction("Index", "Authenticate", new
                    {
                        SAMLRequest = parameter.SAMLRequest,
                        RelayState = parameter.RelayState,
                        area = result.Amr
                    });
                case SingleSignOnActions.REDIRECT:
                    return Redirect(result.Location);
                default:
                    return Content(result.Content, "text/html");
            }
        }
    }
}
