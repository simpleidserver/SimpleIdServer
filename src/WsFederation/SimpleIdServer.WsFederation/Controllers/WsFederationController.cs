// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.WsFederation;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Xml;
using System.Net;
using System.Xml;

namespace SimpleIdServer.WsFederation.Controllers
{
    [Route(Constants.RouteNames.WsFederation)]
    public class WsFederationController : Controller
    {
        private readonly ILogger<WsFederationController> _logger;
        private readonly IUrlHelper _urlHelper;
        private readonly WsFederationOptions _options;

        public WsFederationController(ILogger<WsFederationController> logger, IUrlHelperFactory factory, IActionContextAccessor accessor, IOptions<WsFederationOptions> options)
        {
            _logger = logger;
            _urlHelper = factory.GetUrlHelper(accessor.ActionContext);
            _options = options.Value;
        }

        [HttpGet("metadata")]
        public async Task<IActionResult> GetMetadata()
        {
            _logger.LogInformation("Start WS-Federation get metadata");
            var issuer = Request.GetAbsoluteUriWithVirtualPath(Constants.RouteNames.WsFederation, "metadata");
            var url = _urlHelper.Action(nameof(Passive), GetControllerName(), new { }, HttpContext.Request.Scheme);
            var signingCredentials = new X509SigningCredentials(_options.SigningCertificate.Certificate);
            var configuration = new WsFederationConfiguration
            {
                Issuer = issuer,
                TokenEndpoint = url,
                SigningCredentials = signingCredentials
            };
            configuration.SigningKeys.Add(signingCredentials.Key);
            configuration.KeyInfos.Add(new KeyInfo(_options.SigningCertificate));

            var xml = Serialize(configuration);
            return new ContentResult
            {
                Content = xml,
                ContentType = "application/xml",
                StatusCode = (int)HttpStatusCode.OK
            };

            string Serialize(WsFederationConfiguration configuration)
            {
                using (var sw = new StringWriter())
                {
                    using (var writer = XmlWriter.Create(sw))
                    {
                        var serializer = new WsFederationMetadataSerializer();
                        serializer.WriteMetadata(writer, configuration);
                    }

                    return sw.ToString();
                }
            }
        }

        /// <summary>
        /// Endpoint address that supports the Web (Passive) Requestor protocol.
        /// </summary>
        /// <returns></returns>
        [HttpGet("passive")]
        public async Task<IActionResult> Passive()
        {
            _logger.LogInformation("Start WS-Federation passive request");
            var federationMessage = WsFederationMessage.FromQueryString(Request.QueryString.Value);
            if(!federationMessage.IsSignInMessage && !federationMessage.IsSignOutMessage)
            {
                // RETURN AN ERROR...
                return null;
            }

            if (federationMessage.IsSignInMessage) return ProcessSignInMessage(federationMessage);
            return ProcessSignOutMessage(federationMessage);

            IActionResult ProcessSignInMessage(WsFederationMessage message)
            {
                // Validate signing message.
                return null;
            }

            IActionResult ProcessSignOutMessage(WsFederationMessage message)
            {
                return null;
            }
        }

        private static string GetControllerName() => nameof(WsFederationController).Replace("Controller", "");
    }
}