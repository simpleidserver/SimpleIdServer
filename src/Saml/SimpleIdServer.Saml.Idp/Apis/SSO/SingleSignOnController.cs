// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.Saml.DTOs;
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Helpers;
using SimpleIdServer.Saml.Idp.Apis.SSO;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Idp.SSO.Apis
{
    [Route(Constants.RouteNames.SingleSignOn)]
    public class SingleSignOnController : Controller
    {
        private readonly ISingleSignOnHandler _singleSignOnHandler;
        private readonly SamlIdpOptions _options;

        public SingleSignOnController(
            ISingleSignOnHandler singleSignOnHandler,
            IOptions<SamlIdpOptions> options)
        {
            _singleSignOnHandler = singleSignOnHandler;
            _options = options.Value;
        }

        [HttpGet("Login")]
        public Task<IActionResult> LoginGet([FromQuery] SAMLRequestDto parameter, CancellationToken cancellationToken)
        {
            return InternalLogin(parameter, cancellationToken);
        }

        [HttpPost("Login")]
        public Task<IActionResult> LoginPost([FromBody] SAMLRequestDto parameter, CancellationToken cancellationToken)
        {
            return InternalLogin(parameter, cancellationToken);
        }

        private async Task<IActionResult> InternalLogin(SAMLRequestDto parameter, CancellationToken cancellationToken)
        {
            var nameId = string.Empty;
            if(User != null && User.Claims.Any())
            {
                nameId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            }

            var result = await _singleSignOnHandler.Handle(parameter, nameId, cancellationToken);
            if (result.IsValid)
            {
                var location = await result.RelyingParty.GetAssertionLocation(Saml.Constants.Bindings.HttpRedirect, cancellationToken);
                var uri = new Uri(location);
                var xml = result.Response.SerializeToXmlElement().OuterXml;
                // https://docs.oasis-open.org/security/saml/v2.0/saml-bindings-2.0-os.pdf chapter 3.4.4.1 : HTTP Redirect Binding
                uri = uri.AddParameter("SAMLResponse", Compression.Compress(xml));
                uri = uri.AddParameter("RelayState", parameter.RelayState);
                uri = uri.AddParameter("SigAlg", Saml.Constants.SignatureAlgorithms.RSASHA1);
                var rsa = _options.SigningCertificate.PrivateKey as RSACng;
                var hashed = new byte[0];
                using (var sha1 = SHA1.Create())
                {
                    hashed = sha1.ComputeHash(Encoding.UTF8.GetBytes(uri.Query.TrimStart('?')));
                }

                var signed = rsa.SignHash(hashed, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
                uri = uri.AddParameter("Signature", Convert.ToBase64String(signed));
                return Redirect(uri.ToString());
            }

            return RedirectToAction("Index", "Authenticate", new
            {
                SAMLRequest = parameter.SAMLRequest,
                RelayState = parameter.RelayState,
                area = result.Amr
            });
        }
    }
}
