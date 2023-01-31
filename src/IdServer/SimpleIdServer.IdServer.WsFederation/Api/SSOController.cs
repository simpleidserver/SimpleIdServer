// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.WsFederation;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml;
using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.IdentityModel.Xml;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.WsFederation.Extensions;
using System.Globalization;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Xml;

namespace SimpleIdServer.IdServer.WsFederation.Api
{
    public class SSOController : BaseWsFederationController
    {
        private readonly IClientRepository _clientRepository;
        private readonly IDataProtector _dataProtector;

        public SSOController(IClientRepository clientRepository, IDataProtectionProvider dataProtectionProvider, IOptions<IdServerWsFederationOptions> options, IKeyStore keyStore) : base(options, keyStore)
        {
            _clientRepository = clientRepository;
            _dataProtector = dataProtectionProvider.CreateProtector("Authorization");
        }

        public Task<IActionResult> Login()
        {
            var queryStr = Request.QueryString.Value;
            var federationMessage = WsFederationMessage.FromQueryString(queryStr);
            if (federationMessage.IsSignInMessage)
                return SignIn(federationMessage);

            return null;
        }

        private async Task<IActionResult> SignIn(WsFederationMessage message)
        {

            async Task Validate()
            {
                var client = await _clientRepository.Query().FirstOrDefaultAsync(c => c.ClientId == message.Wtrealm);
                if (client == null)
                    throw new OAuthException(ErrorCodes.INVALID_RP, ErrorMessages.UNKNOWN_RP);

                if (!client.IsWsFederationEnabled())
                    throw new OAuthException(ErrorCodes.INVALID_RP, ErrorMessages.WSFEDERATION_NOT_ENABLED);


            }

            var descriptor = new SecurityTokenDescriptor
            {
                Audience = "coucou",
                IssuedAt = DateTime.UtcNow,
                NotBefore= DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddSeconds(10),
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "user"),
                    new Claim(ClaimTypes.Email, "user@hotmail.fr")
                }),
                Issuer = "http://localhost"
            };
            var handler = new Saml2SecurityTokenHandler();
            var securityToken = handler.CreateToken(descriptor);

            var response = new RequestSecurityTokenResponse
            {
                CreatedAt = securityToken.ValidFrom,
                ExpiresAt = securityToken.ValidTo,
                AppliesTo = "coucou",
                Context = message.Wctx,
                RequestedSecurityToken = securityToken,
                SecurityTokenHandler = handler
            };
            var responseMessage = new WsFederationMessage
            {
                IssuerAddress = message.Wreply,
                Wresult = response.Serialize(),
                Wctx = message.Wctx,
                PostTitle = message.PostTitle,
                Script = message.Script,
                ScriptButtonText = message.ScriptButtonText,
                ScriptDisabledText = message.ScriptDisabledText,
            };

            if (!string.IsNullOrWhiteSpace(message.Script))
            {
                var content = responseMessage.BuildFormPost();
                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = responseMessage.BuildFormPost()
                };
            }

            return Redirect(responseMessage.BuildRedirectUrl());
            /*
            var client = await _clientRepository.Query().FirstOrDefaultAsync(c => c.ClientId == message.Wtrealm);
            if (client == null)
            {
                message.Wres
                message.Wresult
                var response = new SignInResponseMessage();
            }
                return null;

            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            if (User == null || User.Identity == null || User.Identity.IsAuthenticated == false)
            {
                var queryStr = Request.QueryString.Value;
                var returnUrl = $"{issuer}/{WsFederationConstants.EndPoints.SSO}{queryStr}&{AuthorizationRequestParameters.ClientId}={message.Wtrealm}";
                var url = Url.Action("Index", "Authenticate", new
                {
                    returnUrl = _dataProtector.Protect(returnUrl),
                    area = Constants.Areas.Password
                });
                return Redirect(url);
            }


            return null;
            */
        }

        private class RequestSecurityTokenResponse
        {
            public DateTime CreatedAt { get; set; }
            public DateTime ExpiresAt { get; set; }
            public string AppliesTo { get; set; }
            public string Context { get; set; }
            public string ReplyTo { get; set; }
            public SecurityToken RequestedSecurityToken { get; set; }
            public SecurityTokenHandler SecurityTokenHandler { get; set; }

            public string Serialize()
            {
                using var ms = new MemoryStream();
                using var writer = XmlDictionaryWriter.CreateTextWriter(ms, Encoding.UTF8, false);
                // <t:RequestSecurityTokenResponseCollection>
                writer.WriteStartElement(WsTrustConstants_1_3.PreferredPrefix, Microsoft.IdentityModel.Xml.WsTrustConstants.Elements.RequestSecurityTokenResponseCollection, WsTrustConstants.Namespaces.WsTrust1_3);
                // <t:RequestSecurityTokenResponse>
                writer.WriteStartElement(WsTrustConstants_1_3.PreferredPrefix, Microsoft.IdentityModel.Xml.WsTrustConstants.Elements.RequestSecurityTokenResponse, WsTrustConstants.Namespaces.WsTrust1_3);
                // @Context
                writer.WriteAttributeString("Context", Context);

                // <t:Lifetime>
                writer.WriteStartElement(WsTrustConstants.Elements.Lifetime, WsTrustConstants.Namespaces.WsTrust1_3);

                // <wsu:Created></wsu:Created>
                writer.WriteElementString(WsUtility.PreferredPrefix, WsUtility.Elements.Created, WsUtility.Namespace, CreatedAt.ToString(SamlConstants.GeneratedDateTimeFormat, DateTimeFormatInfo.InvariantInfo));
                // <wsu:Expires></wsu:Expires>
                writer.WriteElementString(WsUtility.PreferredPrefix, WsUtility.Elements.Expires, WsUtility.Namespace, ExpiresAt.ToString(SamlConstants.GeneratedDateTimeFormat, DateTimeFormatInfo.InvariantInfo));

                // </t:Lifetime>
                writer.WriteEndElement();

                // <wsp:AppliesTo>
                writer.WriteStartElement(WsPolicy.PreferredPrefix, WsPolicy.Elements.AppliesTo, WsPolicy.Namespace);

                // <wsa:EndpointReference>
                writer.WriteStartElement(WsAddressing.PreferredPrefix, WsAddressing.Elements.EndpointReference, WsAddressing.Namespace);

                // <wsa:Address></wsa:Address>
                writer.WriteElementString(WsAddressing.PreferredPrefix, WsAddressing.Elements.Address, WsAddressing.Namespace, AppliesTo);

                writer.WriteEndElement();
                // </wsa:EndpointReference>

                writer.WriteEndElement();
                // </wsp:AppliesTo>

                // <t:RequestedSecurityToken>
                writer.WriteStartElement(WsTrustConstants_1_3.PreferredPrefix, WsTrustConstants.Elements.RequestedSecurityToken, WsTrustConstants.Namespaces.WsTrust1_3);

                // write assertion
                SecurityTokenHandler.WriteToken(writer, RequestedSecurityToken);

                // </t:RequestedSecurityToken>
                writer.WriteEndElement();

                // </t:RequestSecurityTokenResponse>
                writer.WriteEndElement();

                // <t:RequestSecurityTokenResponseCollection>
                writer.WriteEndElement();

                writer.Flush();
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}
