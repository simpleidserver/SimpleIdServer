// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.Saml.DTOs;
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Helpers;
using SimpleIdServer.Saml.Idp.Apis.SSO;
using SimpleIdServer.Saml.Stores;
using System;
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
        private readonly IEntityDescriptorStore _entityDescriptorStore;
        private readonly SamlIdpOptions _options;

        public SingleSignOnController(
            ISingleSignOnHandler singleSignOnHandler,
            IEntityDescriptorStore entityDescriptorStore,
            IOptions<SamlIdpOptions> options)
        {
            _singleSignOnHandler = singleSignOnHandler;
            _entityDescriptorStore = entityDescriptorStore;
            _options = options.Value;
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
            if (result.IsValid)
            {
                var location = await result.RelyingParty.GetAssertionLocation(_entityDescriptorStore, Saml.Constants.Bindings.HttpRedirect, cancellationToken);
                if (!string.IsNullOrWhiteSpace(location))
                {
                var uri = new Uri(location);
                var redirectionUrl = MessageEncodingBuilder.EncodeHTTPBindingResponse(uri, result.Response.SerializeToXmlElement(), parameter.RelayState, _options.SigningCertificate, _options.SignatureAlg);
                return Redirect(redirectionUrl);

                }
                location = await result.RelyingParty.GetAssertionLocation(_entityDescriptorStore, Saml.Constants.Bindings.HttpPost, cancellationToken);
                if (!string.IsNullOrWhiteSpace(location))
                {
                    var SAMLResponse = MessageEncodingBuilder.EncodeHTTPPostResponse(result.Response.SerializeToXmlElement());
                    return Content(GetPostResultHtml(location, SAMLResponse, parameter.RelayState), "text/html");
                }
            }

            return RedirectToAction("Index", "Authenticate", new
            {
                SAMLRequest = parameter.SAMLRequest,
                RelayState = parameter.RelayState,
                area = result.Amr
            });
        }

        private string GetPostResultHtml(string URL, string SAMLResponse, string RelayState)
        {
            var result =
$@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""utf-8"" />
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />
    <title>SAML 2.0</title>
</head>
<body onload=""document.forms[0].submit()"">
    <noscript>
        <p>
            <strong>Note:</strong> Since your browser does not support JavaScript, 
            you must press the Continue button once to proceed.
        </p>
    </noscript>
    <form action=""{URL}"" method=""post"">
        <div>
        <input type=""hidden"" name=""SAMLResponse""  value=""{SAMLResponse}""/>";

            if (!string.IsNullOrWhiteSpace(RelayState))
            {
                result = result + $@"<input type=""hidden"" name=""RelayState"" value=""{RelayState}""/>";
            }

            return result +
@"</div>
        <noscript>
            <div>
                <input type=""submit"" value=""Continue""/>
            </div>
        </noscript>
    </form>
</body>
</html>";

        }
    }
}
