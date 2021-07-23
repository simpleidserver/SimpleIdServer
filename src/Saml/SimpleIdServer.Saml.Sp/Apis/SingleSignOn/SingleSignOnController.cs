// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.Saml.DTOs;
using SimpleIdServer.Saml.Exceptions;
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Helpers;
using SimpleIdServer.Saml.Sp.Resources;
using SimpleIdServer.Saml.Sp.Validators;
using SimpleIdServer.Saml.Stores;
using SimpleIdServer.Saml.Xsd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Sp.Apis.SingleSignOn
{
    [Route(Saml.Constants.RouteNames.SingleSignOn)]
    public class SingleSignOnController : Controller
    {
        private static Dictionary<string, string> MappingSamlAttributeToClaim = new Dictionary<string, string>
        {
            { "urn:oid:2.5.4.42", ClaimTypes.GivenName }
        };
        private readonly IAuthnRequestGenerator _authnRequestGenerator;
        private readonly IEntityDescriptorStore _entityDescriptorStore;
        private readonly ISAMLResponseValidator _samlResponseValidator;
        private readonly SamlSpOptions _options;

        public SingleSignOnController(
            IAuthnRequestGenerator authnRequestGenerator,
            IEntityDescriptorStore entityDescriptorStore,
            ISAMLResponseValidator samlResponseValidator,
            IOptions<SamlSpOptions> options)
        {
            _authnRequestGenerator = authnRequestGenerator;
            _entityDescriptorStore = entityDescriptorStore;
            _samlResponseValidator = samlResponseValidator;
            _options = options.Value;
        }

        [HttpGet("Login")]
        public async Task<IActionResult> Login(CancellationToken cancellationToken, string returnUrl = "")
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var entityDescriptor = await _entityDescriptorStore.Get(_options.IdpMetadataUrl, cancellationToken);
            var idp = entityDescriptor.Items.FirstOrDefault(i => i is IDPSSODescriptorType) as IDPSSODescriptorType;
            if (idp == null)
            {
                throw new SamlException(System.Net.HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.Requester, Global.BadRelyingPartyIdpMetadata);
            }

            var ssoService = idp.SingleSignOnService.FirstOrDefault(s => s.Binding == Constants.Bindings.HttpRedirect);
            if (ssoService == null)
            {
                throw new SamlException(System.Net.HttpStatusCode.BadRequest, Saml.Constants.StatusCodes.UnsupportedBinding, Global.BadIDPSingleSignOnLocation);
            }

            var authnRequest = _authnRequestGenerator.BuildHttpGetBinding();
            var uri = new Uri(ssoService.Location);
            returnUrl = string.IsNullOrWhiteSpace(returnUrl) ? issuer : returnUrl;
            var redirectionUrl = MessageEncodingBuilder.EncodeHTTPBindingRequest(uri, authnRequest, returnUrl, _options.SigningCertificate, _options.SignatureAlg);
            return Redirect(redirectionUrl);
        }

        [HttpGet("AssertionConsumer")]
        public async Task<IActionResult> AssertionConsumer(CancellationToken cancellationToken)
        {
            var samlResponse = SAMLResponseDto.Build(Request.Query.ToDictionary(k => k.Key, k => k.Value.First()));
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var assertion = await _samlResponseValidator.Validate(samlResponse, cancellationToken);
            var claimsIdentity = new ClaimsIdentity(BuildClaims(assertion), "saml");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
            var redirectionUrl = string.IsNullOrWhiteSpace(samlResponse.RelayState) ? issuer : samlResponse.RelayState;
            return Redirect(redirectionUrl);
        }

        private static ICollection<Claim> BuildClaims(AssertionType assertion)
        {
            var claims = new List<Claim>();
            var nameId = assertion.Subject.Items.FirstOrDefault(i => i is NameIDType) as NameIDType;
            if (nameId != null)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, nameId.Value));
            }

            if (assertion.Items != null)
            {
                var attributeStatement = assertion.Items.FirstOrDefault(i => i is AttributeStatementType) as AttributeStatementType;
                if (attributeStatement != null)
                {
                    var attributes = attributeStatement.Items.Where(i => i is AttributeType).Cast<AttributeType>();
                    foreach (var attribute in attributes)
                    {
                        string name = attribute.Name;
                        if (MappingSamlAttributeToClaim.ContainsKey(attribute.Name))
                        {
                            name = MappingSamlAttributeToClaim[attribute.Name];
                        }

                        foreach (var attributeValue in attribute.AttributeValue.Where(a => a is AttributeValueType).Cast<AttributeValueType>())
                        {
                            claims.Add(new Claim(name, attributeValue.Value));
                        }
                    }
                }
            }

            return claims;
        }
    }
}
