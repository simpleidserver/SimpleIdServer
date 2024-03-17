// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QRCoder;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.VerifiablePresentation.Resources;
using System.Net;
using System.Text.Json;
using System.Web;

namespace SimpleIdServer.IdServer.VerifiablePresentation.Apis;

public class AuthorizationController : BaseController
{
    private readonly IPresentationDefinitionStore _presentationDefinitionStore;
    private readonly IDistributedCache _distributedCache;
    private readonly VerifiablePresentationOptions _options;
    private readonly ILogger<AuthorizationController> _logger;

    public AuthorizationController(
        IPresentationDefinitionStore presentationDefinitionStore,
        IDistributedCache distributedCache,
        IOptions<VerifiablePresentationOptions> options,
        ILogger<AuthorizationController> logger,
        ITokenRepository tokenRepository, 
        IJwtBuilder jwtBuilder) : base(tokenRepository, jwtBuilder)
    {
        _presentationDefinitionStore = presentationDefinitionStore;
        _distributedCache = distributedCache;
        _options = options.Value;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Callback([FromRoute] string prefix, [FromForm] VpAuthorizationResponse request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? IdServer.Constants.Prefix;
        try
        {
            // CONTINUE : https://openid.net/specs/openid-4-verifiable-presentations-1_0.html#name-response-mode-direct_post
            await Validate(request, cancellationToken);
            return null;
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetQRCode([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        prefix = prefix ?? IdServer.Constants.Prefix;
        try
        {
            var presentationDefinition = await _presentationDefinitionStore.Query()
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.PublicId == id && p.RealmName == prefix, cancellationToken);
            if(presentationDefinition == null)
                throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownPresentationDefinition, id));
            var state = Guid.NewGuid().ToString();
            var nonce = Guid.NewGuid().ToString();
            var qrCodeUrl = GetQRCodeUrl(id, prefix);
            await _distributedCache.SetStringAsync(state, JsonSerializer.Serialize(new VpPendingAuthorization(id, state, nonce)), new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMilliseconds(_options.SlidingExpirationTimeVpOfferMs)
            }, cancellationToken);
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(qrCodeUrl, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            var payload = qrCode.GetGraphic(20);
            return File(payload, "image/png");
        }
        catch(OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    private string GetQRCodeUrl(string presentationDefinitionId, string prefix)
    {
        var authorizationRequest = BuildAuthorizationRequestDirectPost(presentationDefinitionId, prefix);
        var json = JsonSerializer.Serialize(authorizationRequest);
        var encodedJson = HttpUtility.UrlEncode(json);
        return $"openid4vp://authorize?{encodedJson}";
    }

    private VpAuthorizationRequest BuildAuthorizationRequestDirectPost(string presentationDefinitionId, string prefix)
    {
        var issuer = Request.GetAbsoluteUriWithVirtualPath();
        var result = new VpAuthorizationRequest
        {
            Nonce = Guid.NewGuid().ToString(),
            ResponseMode = "direct_post",
            ResponseUri = $"{issuer}/{prefix}{Constants.Endpoints.VpAuthorizeCallback}",
            ResponseType = "vp_token",
            State = Guid.NewGuid().ToString(),
            PresentationDefinitionUri = $"{issuer}/{prefix}/{Constants.Endpoints.PresentationDefinitions}/{presentationDefinitionId}"
        };
        return result;
    }        

    private async Task Validate(VpAuthorizationResponse request, CancellationToken cancellationToken)
    {
        if (request == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.InvalidIncomingRequest);
        if (string.IsNullOrWhiteSpace(request.VpToken)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, "vp_token"));
        if (string.IsNullOrWhiteSpace(request.PresentationSubmission)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, "presentation_submission"));
        if (string.IsNullOrWhiteSpace(request.State)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, "state"));

        var verifiablePresentation = JsonSerializer.Deserialize<Vp.Models.VerifiablePresentation>(request.VpToken);
        if (verifiablePresentation == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.InvalidVerifiablePresentation);
        var presentationSubmission = JsonSerializer.Deserialize<PresentationSubmission>(request.PresentationSubmission);
        if (presentationSubmission == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.InvalidPresentationSubmission);

        var cachedValue = await _distributedCache.GetStringAsync(request.State, cancellationToken);
        if (cachedValue == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.StateIsNotValid);
        var vpPendingAuthorization = JsonSerializer.Deserialize<VpPendingAuthorization>(cachedValue);
    }

    private class VpPendingAuthorization
    {
        public VpPendingAuthorization(string presentationDefinitionId, string state, string nonce)
        {
            PresentationDefinitionId = presentationDefinitionId;
            State = state;
            Nonce = nonce;
        }

        public string PresentationDefinitionId { get; set; }
        public string State { get; set; }
        public string Nonce { get; set; }
    }
}
