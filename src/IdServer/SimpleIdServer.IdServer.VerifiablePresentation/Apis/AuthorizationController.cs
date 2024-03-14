// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<AuthorizationController> _logger;

    public AuthorizationController(
        IPresentationDefinitionStore presentationDefinitionStore,
        ILogger<AuthorizationController> logger,
        ITokenRepository tokenRepository, 
        IJwtBuilder jwtBuilder) : base(tokenRepository, jwtBuilder)
    {
        _presentationDefinitionStore = presentationDefinitionStore;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Callback([FromRoute] string prefix, CancellationToken cancellationToken)
    {
        prefix = prefix ?? IdServer.Constants.Prefix;
        try
        {
            // CONTINUE : https://openid.net/specs/openid-4-verifiable-presentations-1_0.html#name-response-mode-direct_post
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
            var qrCodeUrl = GetQRCodeUrl(id, prefix);
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
}
