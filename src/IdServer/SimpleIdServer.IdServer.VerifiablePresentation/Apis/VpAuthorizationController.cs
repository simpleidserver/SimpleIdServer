// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using BlushingPenguin.JsonPath;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QRCoder;
using SimpleIdServer.Did;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.VerifiablePresentation.DTOs;
using SimpleIdServer.IdServer.VerifiablePresentation.Resources;
using SimpleIdServer.Vc;
using SimpleIdServer.Vc.Models;
using SimpleIdServer.Vp;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;

namespace SimpleIdServer.IdServer.VerifiablePresentation.Apis;

public class VpAuthorizationController : BaseController
{
    private readonly IdServerHostOptions _idServerOptions;
    private readonly IPresentationDefinitionStore _presentationDefinitionStore;
    private readonly IDistributedCache _distributedCache;
    private readonly VerifiablePresentationOptions _options;
    private readonly IDidFactoryResolver _didFactoryResolver;
    private readonly ILogger<VpAuthorizationController> _logger;

    public VpAuthorizationController(
        IOptions<IdServerHostOptions> idServerOptions,
        IPresentationDefinitionStore presentationDefinitionStore,
        IDistributedCache distributedCache,
        IOptions<VerifiablePresentationOptions> options,
        ILogger<VpAuthorizationController> logger,
        ITokenRepository tokenRepository, 
        IJwtBuilder jwtBuilder,
        IDidFactoryResolver didFactoryResolver) : base(tokenRepository, jwtBuilder)
    {
        _idServerOptions = idServerOptions.Value;
        _presentationDefinitionStore = presentationDefinitionStore;
        _distributedCache = distributedCache;
        _options = options.Value;
        _didFactoryResolver = didFactoryResolver;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Callback([FromRoute] string prefix, [FromForm] VpAuthorizationResponse request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? IdServer.Constants.DefaultRealm;
        try
        {
            var validationResult = await Validate(request, prefix, cancellationToken);
            var pendingAuthorization = validationResult.PendingAuthorization;
            pendingAuthorization.IsAuthorized = true;
            pendingAuthorization.VcSubjects = validationResult.VcSubjects;
            await _distributedCache.SetStringAsync(request.State, JsonSerializer.Serialize(pendingAuthorization), new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMilliseconds(_options.SlidingExpirationTimeVpOfferMs)
            }, cancellationToken);
            return new NoContentResult();
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
        prefix = prefix ?? IdServer.Constants.DefaultRealm;
        try
        {
            var presentationDefinition = await _presentationDefinitionStore.Query()
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.PublicId == id && p.RealmName == prefix, cancellationToken);
            if(presentationDefinition == null)
                throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownPresentationDefinition, id));
            var state = Guid.NewGuid().ToString();
            var nonce = Guid.NewGuid().ToString();
            var qrCodeUrl = GetQRCodeUrl(id, prefix, nonce, state);
            await _distributedCache.SetStringAsync(state, JsonSerializer.Serialize(new VpPendingAuthorization(id, state, nonce)), new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMilliseconds(_options.SlidingExpirationTimeVpOfferMs)
            }, cancellationToken);
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(qrCodeUrl, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            var payload = qrCode.GetGraphic(20);
            Response.Headers.Add("State", state);
            return File(payload, "image/png");
        }
        catch(OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    private string GetQRCodeUrl(string presentationDefinitionId, string prefix, string nonce, string state)
    {
        var authorizationRequest = BuildAuthorizationRequestDirectPost(presentationDefinitionId, prefix, nonce, state);
        var json = JsonSerializer.Serialize(authorizationRequest);
        var encodedJson = HttpUtility.UrlEncode(json);
        return $"openid4vp://authorize?{encodedJson}";
    }

    private VpAuthorizationRequest BuildAuthorizationRequestDirectPost(string presentationDefinitionId, string prefix, string nonce, string state)
    {
        var issuer = Request.GetAbsoluteUriWithVirtualPath();
        var result = new VpAuthorizationRequest
        {
            Nonce = nonce,
            ResponseMode = "direct_post",
            ResponseUri = $"{issuer}/{GetRealm(prefix)}{Constants.Endpoints.VpAuthorizeCallback}",
            ResponseType = "vp_token",
            State = state,
            PresentationDefinitionUri = $"{issuer}/{GetRealm(prefix)}/{Constants.Endpoints.PresentationDefinitions}/{presentationDefinitionId}"
        };
        return result;
    }

    private string GetRealm(string realm) => _idServerOptions.UseRealm ? $"{realm}/" : string.Empty;

    private async Task<VpAuthorizationValidationResult> Validate(VpAuthorizationResponse request, string prefix, CancellationToken cancellationToken)
    {
        if (request == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.InvalidIncomingRequest);
        if (string.IsNullOrWhiteSpace(request.VpToken)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, "vp_token"));
        if (string.IsNullOrWhiteSpace(request.PresentationSubmission)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, "presentation_submission"));
        if (string.IsNullOrWhiteSpace(request.State)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, "state"));

        var verifiablePresentation = JsonSerializer.Deserialize<Vp.Models.VerifiablePresentation>(request.VpToken);
        if (verifiablePresentation == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.InvalidVerifiablePresentation);
        var presentationSubmission = JsonSerializer.Deserialize<PresentationSubmission>(request.PresentationSubmission);
        if (presentationSubmission == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.InvalidPresentationSubmission);
        var verifiablePresentationJsonObject = JsonDocument.Parse(request.VpToken);

        var cachedValue = await _distributedCache.GetStringAsync(request.State, cancellationToken);
        if (cachedValue == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.StateIsNotValid);
        var vpPendingAuthorization = JsonSerializer.Deserialize<VpPendingAuthorization>(cachedValue);
        var presentationDefinition = await _presentationDefinitionStore.Query()
            .Include(p => p.InputDescriptors).ThenInclude(p => p.Format)
            .Include(p => p.InputDescriptors).ThenInclude(p => p.Constraints)
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.PublicId == vpPendingAuthorization.PresentationDefinitionId && p.RealmName == prefix, cancellationToken);

        var vpVerifier = new VpVerifier(_didFactoryResolver);
        try
        {
            await vpVerifier.Verify(verifiablePresentation, cancellationToken);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.ToString());
            throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.VerifiablePresentationProofInvalid);
        }

        var result = new Dictionary<string, JsonNode>();
        var securedDocument = SecuredDocument.New();
        var inputDescriptors = presentationDefinition.InputDescriptors;
        foreach(var inputDescriptor in inputDescriptors)
        {
            var descriptorMap = presentationSubmission.DescriptorMaps.SingleOrDefault(m => m.Id == inputDescriptor.PublicId);
            if (descriptorMap == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.PresentationSubmissionMissingVerifiableCredential, inputDescriptor.PublicId));
            if (descriptorMap.PathNested == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.PresentationSubmissinMissingPathNested);
            if (!inputDescriptor.Format.Any(f => f.Format == descriptorMap.PathNested.Format)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.PresentationSubmissionBadFormat, descriptorMap.PathNested.Format));
            var token = verifiablePresentationJsonObject.SelectToken(descriptorMap.PathNested.Path);
            if (token == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.CannotExtractVcFromPath, descriptorMap.PathNested.Path));
            var vcJson = token.ToString();
            var vc = JsonSerializer.Deserialize<W3CVerifiableCredential>(vcJson);
            if (vc == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.InvalidVerifiableCredential);
            var did = await _didFactoryResolver.Resolve(vc.Issuer, cancellationToken);
            if (did == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.VcIssuerNotDid);
            if (!securedDocument.Check(vc, did)) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.VerifiableCredentialProofInvalid, vc.Id));
            result.Add(vc.Id, vc.CredentialSubject);
        }

        return new VpAuthorizationValidationResult
        {
            PendingAuthorization = vpPendingAuthorization,
            VcSubjects = result
        };
    }
    
    private class VpAuthorizationValidationResult
    {
        public VpPendingAuthorization PendingAuthorization { get; set; }
        public Dictionary<string, JsonNode> VcSubjects { get; set; }
    }
}
