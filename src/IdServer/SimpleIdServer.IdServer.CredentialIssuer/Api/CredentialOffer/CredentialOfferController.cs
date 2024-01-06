// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QRCoder;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.CredentialIssuer.Api.CredentialOffer;

public class CredentialOfferController : BaseController
{
    private readonly ICredentialTemplateRepository _credentialTemplateRepository;
    private readonly ICredentialOfferRepository _credentialOfferRepository;
    private readonly IAuthenticationHelper _authenticationHelper;
    private readonly IClientRepository _clientRepository;
    private readonly IEnumerable<IUserNotificationService> _notificationServices;
    private readonly IGrantedTokenHelper _grantedTokenHelper;
    private readonly UrlEncoder _urlEncoder;
    private readonly IdServerHostOptions _options;

    public CredentialOfferController(
        ICredentialTemplateRepository credentialTemplateRepository, 
        ICredentialOfferRepository credentialOfferRepository, 
        IAuthenticationHelper authenticationHelper, 
        IClientRepository clientRepository,
        IEnumerable<IUserNotificationService> notificationServices, 
        IGrantedTokenHelper grantedTokenHelper, 
        ITokenRepository tokenRepository,
        IJwtBuilder jwtBuilder,
        UrlEncoder urlEncoder, 
        IOptions<IdServerHostOptions> options) : base(tokenRepository, jwtBuilder)
    {
        _credentialTemplateRepository = credentialTemplateRepository;
        _credentialOfferRepository = credentialOfferRepository;
        _authenticationHelper = authenticationHelper;
        _clientRepository = clientRepository;
        _notificationServices = notificationServices;
        _grantedTokenHelper = grantedTokenHelper;
        _urlEncoder = urlEncoder;
        _options = options.Value;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ClientShareQR([FromRoute] string prefix, string id, [FromBody] ShareCredentialTemplateRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? SimpleIdServer.IdServer.Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, IdServer.Constants.StandardScopes.CredentialOffer.Name);
            var kvp = await InternalShare(prefix, id, request, cancellationToken);
            if (kvp.Item1 != null) return kvp.Item1;
            return GetQRCode(kvp.Item2);
        }
        catch (OAuthException ex)
        {
            return BuildError(ex);
        }
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ClientShare([FromRoute] string prefix, string id, [FromBody] ShareCredentialTemplateRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? SimpleIdServer.IdServer.Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, IdServer.Constants.StandardScopes.CredentialOffer.Name);
            var kvp = await InternalShare(prefix, id, request, cancellationToken);
            if (kvp.Item1 != null) return kvp.Item1;
            return Redirect(kvp.Item2.Url);
        }
        catch (OAuthException ex)
        {
            return BuildError(ex);
        }
    }

    [HttpPost]
    [Authorize(IdServer.Constants.Policies.Authenticated)]
    public async Task<IActionResult> ShareQR([FromRoute] string prefix, [FromBody] ShareCredentialTemplateRequest request, CancellationToken cancellationToken)
    {
        var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var kvp = await InternalShare(prefix, nameIdentifier, request, cancellationToken);
        if (kvp.Item1 != null) return kvp.Item1;
        return GetQRCode(kvp.Item2);
    }

    [HttpPost]
    [Authorize(IdServer.Constants.Policies.Authenticated)]
    public async Task<IActionResult> Share([FromRoute] string prefix, [FromBody] ShareCredentialTemplateRequest request, CancellationToken cancellationToken)
    {
        var nameIdentifier = User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var kvp = await InternalShare(prefix, nameIdentifier, request, cancellationToken);
        if (kvp.Item1 != null) return kvp.Item1;
        return Redirect(kvp.Item2.Url);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetQRCode([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await InternalGet(prefix, id, cancellationToken);
            if (result.Code != HttpStatusCode.OK) return BuildError(result.Code, result.ErrorCode, result.ErrorMessage);
            return GetQRCode(result);
        }
        catch (OAuthException ex)
        {
            return BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await InternalGet(prefix, id, cancellationToken);
            if (result.Code != HttpStatusCode.OK) return BuildError(result.Code, result.ErrorCode, result.ErrorMessage);
            return Redirect(result.Url);
        }
        catch (OAuthException ex)
        {
            return BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
        }
    }

    private async Task<CredentialOfferBuildResult> InternalGet(string prefix, string id, CancellationToken cancellationToken)
    {
        prefix = prefix ?? SimpleIdServer.IdServer.Constants.DefaultRealm;
        var credentialOffer = await _credentialOfferRepository.Query().AsNoTracking().SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (credentialOffer == null) return CredentialOfferBuildResult.NotFound(ErrorCodes.INVALID_REQUEST,  ErrorMessages.UNKNOWN_CREDENTIAL_OFFER);
        if (credentialOffer.Status == UserCredentialOfferStatus.INVALID) return CredentialOfferBuildResult.Invalid(ErrorCodes.INVALID_CREDOFFER, ErrorMessages.CREDOFFER_IS_INVALID);
        if (credentialOffer.ExpirationDateTime <= DateTime.UtcNow) return CredentialOfferBuildResult.Invalid(ErrorCodes.INVALID_CREDOFFER, ErrorMessages.CREDOFFER_IS_EXPIRED);
        var client = await _clientRepository.Query().Include(c => c.Realms).FirstAsync(c => c.ClientId == credentialOffer.ClientId && c.Realms.Any(r => r.Name == prefix), cancellationToken);
        var issuer = HandlerContext.GetIssuer(Request.GetAbsoluteUriWithVirtualPath(), _options.UseRealm);
        return InternalGet(prefix, credentialOffer, client);
    }

    private CredentialOfferBuildResult InternalGet(string prefix, UserCredentialOffer credentialOffer, Client client)
    {
        prefix = prefix ?? SimpleIdServer.IdServer.Constants.DefaultRealm;
        var issuer = HandlerContext.GetIssuer(Request.GetAbsoluteUriWithVirtualPath(), _options.UseRealm);
        var result = new CredentialOfferResult
        {
            CredentialIssuer = issuer,
            Credentials = credentialOffer.CredentialNames,
            Grants = new Dictionary<string, object>()
        };
        if (client.GrantTypes.Contains(PreAuthorizedCodeHandler.GRANT_TYPE))
            result.Grants.Add(PreAuthorizedCodeHandler.GRANT_TYPE, new Dictionary<string, object>
            {
                { CredentialOfferResultNames.UserPinRequired, client.UserPinRequired },
                { CredentialOfferResultNames.PreAuthorizedCode, credentialOffer.PreAuthorizedCode }
            });

        if (client.GrantTypes.Contains(AuthorizationCodeHandler.GRANT_TYPE))
        {
            result.Grants.Add(AuthorizationCodeHandler.GRANT_TYPE, new Dictionary<string, object>
            {
                { CredentialOfferResultNames.IssuerState, credentialOffer.CredIssuerState }
            });
        }

        var url = BuildUrl(result, client);
        return CredentialOfferBuildResult.Ok(url, credentialOffer);
    }

    private async Task<(IActionResult, CredentialOfferBuildResult)> InternalShare(string prefix, string nameIdentifier, ShareCredentialTemplateRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? IdServer.Constants.DefaultRealm;
        if (request == null) return (BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.MALFROMED_INCOMING_REQUEST), null);
        if (string.IsNullOrWhiteSpace(request.WalletClientId)) return (BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, ShareCredentialTemplateNames.WalletClientId)), null);
        if (string.IsNullOrWhiteSpace(request.CredentialTemplateId)) return (BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, ShareCredentialTemplateNames.CredentialTemplateId)), null);
        var credentialTemplate = await _credentialTemplateRepository.Query().AsNoTracking().SingleOrDefaultAsync(c => c.TechnicalId == request.CredentialTemplateId, cancellationToken);
        if (credentialTemplate == null) return (BuildError(HttpStatusCode.NotFound, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_TEMPLATE, request.CredentialTemplateId)), null);
        var client = await _clientRepository.Query().AsNoTracking().SingleOrDefaultAsync(c => c.ClientId == request.WalletClientId && c.ClientType == ClientTypes.WALLET, cancellationToken);
        if (client == null) return (BuildError(HttpStatusCode.NotFound, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_WALLET_CLIENT_ID, request.WalletClientId)), null);
        var user = await _authenticationHelper.GetUserByLogin(nameIdentifier, prefix, cancellationToken);
        if (user == null) return (BuildError(HttpStatusCode.Unauthorized, ErrorCodes.UNAUTHORIZED, string.Format(ErrorMessages.UNKNOWN_USER, nameIdentifier)), null);
        var cred = await _credentialOfferRepository.Query().AsNoTracking().FirstOrDefaultAsync(c => c.CredentialTemplateId == request.CredentialTemplateId && c.UserId == user.Id && c.ClientId == request.WalletClientId);

        if (cred == null || cred.Status != UserCredentialOfferStatus.VALID || cred.ExpirationDateTime <= DateTime.UtcNow)
        {
            cred = new UserCredentialOffer
            {
                Id = Guid.NewGuid().ToString(),
                UpdateDateTime = DateTime.UtcNow,
                CreateDateTime = DateTime.UtcNow,
                ExpirationDateTime = DateTime.UtcNow.AddSeconds(_options.CredOfferExpirationInSeconds),
                Status = UserCredentialOfferStatus.VALID,
                ClientId = request.WalletClientId,
                UserId = user.Id,
                CredentialTemplateId = request.CredentialTemplateId
            };
            _credentialOfferRepository.Add(cred);
            if (client.GrantTypes.Contains(PreAuthorizedCodeHandler.GRANT_TYPE))
            {
                string pin = string.Empty;
                if (client.UserPinRequired)
                {
                    pin = Guid.NewGuid().ToString();
                    var notificationService = _notificationServices.First(n => (user.NotificationMode ?? IdServer.Constants.DefaultNotificationMode) == n.Name);
                    await notificationService.Send("PIN", $"The pin is {pin}", new Dictionary<string, string>(), user);
                }

                var preAuthorizedCode = Guid.NewGuid().ToString();
                cred.PreAuthorizedCode = preAuthorizedCode;
                cred.Pin = pin;
                await _grantedTokenHelper.AddPreAuthCode(cred.PreAuthorizedCode, cred.Pin, cred.ClientId, cred.UserId, _options.CredOfferExpirationInSeconds, cancellationToken);
            }

            if (client.GrantTypes.Contains(AuthorizationCodeHandler.GRANT_TYPE))
            {
                var credIssuerState = Guid.NewGuid().ToString();
                cred.CredIssuerState = credIssuerState;
            }

            await _credentialOfferRepository.SaveChanges(cancellationToken);
        }

        var result = InternalGet(prefix, cred, client);
        return (null, result);
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

    private FileContentResult GetQRCode(CredentialOfferBuildResult res)
    {
        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(res.Url, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrCodeData);
        var payload = qrCode.GetGraphic(20);
        return File(payload, "image/png");
    }

    private class CredentialOfferBuildResult
    {
        private CredentialOfferBuildResult() { }

        public string Url { get; private set; }
        public UserCredentialOffer CredentialOffer { get; private set; }
        public HttpStatusCode Code { get; private set; }
        public string ErrorCode { get; private set; }
        public string ErrorMessage { get; private set; }

        public static CredentialOfferBuildResult Ok(string url, UserCredentialOffer credentialOffer)
        {
            return new CredentialOfferBuildResult
            {
                Code = HttpStatusCode.OK,
                Url = url,
                CredentialOffer = credentialOffer
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

        public static CredentialOfferBuildResult Invalid(string errorCode, string errorMessage) 
        {
            return new CredentialOfferBuildResult
            {
                Code = HttpStatusCode.InternalServerError,
                ErrorCode= errorCode,
                ErrorMessage = errorMessage
            };
        }
    }
}
