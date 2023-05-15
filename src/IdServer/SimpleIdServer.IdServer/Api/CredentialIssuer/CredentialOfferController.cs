// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using QRCoder;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Extensions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.CredentialIssuer
{
    public class CredentialOfferController : BaseController
    {
        private readonly ICredentialOfferRepository _credentialOfferRepository;
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IClientRepository _clientRepository;
        private readonly IEnumerable<IUserNotificationService> _notificationServices;
        private readonly UrlEncoder _urlEncoder;
        private readonly IdServerHostOptions _options;

        public CredentialOfferController(ICredentialOfferRepository credentialOfferRepository, IGrantedTokenHelper grantedTokenHelper, IClientRepository clientRepository, IEnumerable<IUserNotificationService> notificationServices, UrlEncoder urlEncoder, IOptions<IdServerHostOptions> options)
        {
            _credentialOfferRepository = credentialOfferRepository;
            _grantedTokenHelper = grantedTokenHelper;
            _clientRepository = clientRepository;
            _notificationServices = notificationServices;
            _urlEncoder = urlEncoder;
            _options = options.Value;
        }

        [HttpGet]
        public async Task<IActionResult> GetQRCode([FromRoute] string prefix, string id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await InternalGet(prefix, id, cancellationToken);
                if (result.Code != HttpStatusCode.OK) return BuildError(result.Code, result.ErrorCode, result.ErrorMessage);
                var qrCode = GetQRCode(result);
                byte[] payload = null;
                using (var stream = new MemoryStream())
                {
                    qrCode.Save(stream, ImageFormat.Png);
                    payload = stream.ToArray();
                }

                return File(payload, "image/png");
            }
            catch (OAuthException ex)
            {
                return BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }

            Bitmap GetQRCode(CredentialOfferBuildResult res)
            {
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(res.Url, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCode(qrCodeData);
                return qrCode.GetGraphic(20);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromRoute] string prefix, string id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await InternalGet(prefix, id, cancellationToken);
                if (result.Code != HttpStatusCode.OK) return BuildError(result.Code, result.ErrorCode, result.ErrorMessage);
                return Ok(result.Url);
            }
            catch(OAuthException ex)
            {
                return BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }

        private async Task<CredentialOfferBuildResult> InternalGet(string prefix, string id, CancellationToken cancellationToken)
        {
            // https://openid.bitbucket.io/connect/openid-4-verifiable-credential-issuance-1_0.html#section-4.1.1
            prefix = prefix ?? Constants.DefaultRealm;
            var bearerToken = ExtractBearerToken();
            var token = await _grantedTokenHelper.GetAccessToken(bearerToken, cancellationToken);
            if (token == null) return CredentialOfferBuildResult.Unauthorized(ErrorCodes.INVALID_TOKEN, ErrorMessages.UNKNOWN_ACCESS_TOKEN);
            var scopes = token.Claims.Where(c => c.Type == "scope").Select(c => c.Value).ToList();
            if (!scopes.Contains(Constants.StandardScopes.CredentialOffer.Name)) return CredentialOfferBuildResult.Unauthorized(ErrorCodes.INVALID_TOKEN, ErrorMessages.INVALID_ACCESS_TOKEN_SCOPE);

            var credentialOffer = await _credentialOfferRepository.Query().Include(c => c.User).SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
            if (credentialOffer == null) return CredentialOfferBuildResult.NotFound(ErrorCodes.INVALID_REQUEST,  ErrorMessages.UNKNOWN_CREDENTIAL_OFFER);

            var clientId = token.Claims.FirstOrDefault(c => c.Type == OpenIdConnectParameterNames.ClientId)?.Value;
            var client = await _clientRepository.Query().Include(c => c.Realms).FirstAsync(c => c.ClientId == clientId && c.Realms.Any(r => r.Name == prefix), cancellationToken);
            var issuer = HandlerContext.GetIssuer(Request.GetAbsoluteUriWithVirtualPath());
            var result = new CredentialOfferResult
            {
                CredentialIssuer = issuer,
                Credentials = credentialOffer.CredentialNames,
                Grants = new Dictionary<string, object>()
            };
            if (client.GrantTypes.Contains(PreAuthorizedCodeHandler.GRANT_TYPE))
            {
                string pin = string.Empty;
                if(client.UserPinRequired)
                {
                    pin = Guid.NewGuid().ToString();
                    var notificationService = _notificationServices.First(n => (credentialOffer.User.NotificationMode ?? Constants.DefaultNotificationMode) == n.Name);
                    await notificationService.Send(pin, credentialOffer.User);
                }

                var preAuthorizedCode = Guid.NewGuid().ToString();
                await _grantedTokenHelper.AddPreAuthorizationCode(preAuthorizedCode, pin, _options.PreAuthorizationCodeExpirationInSeconds, cancellationToken);
                result.Grants.Add(PreAuthorizedCodeHandler.GRANT_TYPE, new Dictionary<string, object>
                {
                    { CredentialOfferResultNames.UserPinRequired, client.UserPinRequired },
                    { CredentialOfferResultNames.PreAuthorizedCode, Guid.NewGuid().ToString() }
                });
            }

            if (client.GrantTypes.Contains(AuthorizationCodeHandler.GRANT_TYPE))
            {
                var issuerState = Guid.NewGuid().ToString();
                await _grantedTokenHelper.AddAuthorizationCodeIssuerState(credentialOffer.Id, issuerState, clientId, _options.AuthorizationCodeIssuerStateExpirationInSeconds, cancellationToken);
                result.Grants.Add(AuthorizationCodeHandler.GRANT_TYPE, new Dictionary<string, object>
                {
                    { CredentialOfferResultNames.IssuerState, issuerState }
                });
            }

            var url = BuildUrl(result, client);
            return CredentialOfferBuildResult.Ok(url);
        }

        private string BuildUrl(CredentialOfferResult result, Client client)
        {
            var res = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(CredentialOfferResultNames.CredentialOffer, _urlEncoder.Encode(JsonSerializer.Serialize(result)))
            };
            var queryCollection = new QueryBuilder(res);
            if (!string.IsNullOrWhiteSpace(client.CredentialOfferEndpoint))
                return $"{client.CredentialOfferEndpoint}{queryCollection.ToQueryString()}";

            return $"openid-credential-offer://{queryCollection.ToQueryString()}";
        }

        private class CredentialOfferBuildResult
        {
            private CredentialOfferBuildResult() { }

            public string Url { get; private set; }
            public HttpStatusCode Code { get; private set; }
            public string ErrorCode { get; private set; }
            public string ErrorMessage { get; private set; }

            public static CredentialOfferBuildResult Ok(string url)
            {
                return new CredentialOfferBuildResult
                {
                    Code = HttpStatusCode.OK,
                    Url = url
                };
            }

            public static CredentialOfferBuildResult Unauthorized(string errorCode, string errorMessage)
            {
                return new CredentialOfferBuildResult
                {
                    Code = HttpStatusCode.Unauthorized,
                    ErrorCode = errorCode,
                    ErrorMessage = errorMessage
                };
            }

            public static CredentialOfferBuildResult NotFound(string errorCode, string errorMessage)
            {
                return new CredentialOfferBuildResult
                {
                    Code = HttpStatusCode.NotFound,
                    ErrorCode = errorCode,
                    ErrorMessage = errorMessage
                };
            }
        }
    }
}
