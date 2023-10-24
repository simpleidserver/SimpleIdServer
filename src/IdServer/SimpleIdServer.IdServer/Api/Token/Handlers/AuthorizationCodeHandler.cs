// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.DPoP;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Api.Token.TokenBuilders;
using SimpleIdServer.IdServer.Api.Token.TokenProfiles;
using SimpleIdServer.IdServer.Api.Token.Validators;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.Handlers
{
    public class AuthorizationCodeHandler : BaseCredentialsHandler
    {
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IAuthorizationCodeGrantTypeValidator _authorizationCodeGrantTypeValidator;
        private readonly IEnumerable<ITokenBuilder> _tokenBuilders;
        private readonly IUserRepository _userRepository;
        private readonly IGrantHelper _audienceHelper;
        private readonly IBusControl _busControl;
        private readonly IDPOPProofValidator _dpopProofValidator;
        private readonly IUserClaimsService _userClaimsService;
        private readonly ILogger<AuthorizationCodeHandler> _logger;

        public AuthorizationCodeHandler(
            IGrantedTokenHelper grantedTokenHelper,
            IAuthorizationCodeGrantTypeValidator authorizationCodeGrantTypeValidator, 
            IEnumerable<ITokenBuilder> tokenBuilders,
            IUserRepository usrRepository,
            IClientAuthenticationHelper clientAuthenticationHelper,
            IGrantHelper audienceHelper,
            IBusControl busControl,
            IDPOPProofValidator dpopProofValidator,
            IOptions<IdServerHostOptions> options,
            IEnumerable<ITokenProfile> tokenProfiles,
            ILogger<AuthorizationCodeHandler> logger,
            IUserClaimsService userClaimsService) : base(clientAuthenticationHelper, tokenProfiles, options)
        {
            _grantedTokenHelper = grantedTokenHelper;
            _authorizationCodeGrantTypeValidator = authorizationCodeGrantTypeValidator;
            _tokenBuilders = tokenBuilders;
            _userRepository = usrRepository;
            _audienceHelper = audienceHelper;
            _busControl = busControl;
            _dpopProofValidator = dpopProofValidator;
            _logger = logger;
            _userClaimsService = userClaimsService;
        }

        public override string GrantType => GRANT_TYPE;
        public static string GRANT_TYPE = "authorization_code";

        public override async Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken)
        {
            IEnumerable<string> scopeLst = new string[0];
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Get Token"))
            {
                try
                {
                    activity?.SetTag("grant_type", GRANT_TYPE);
                    activity?.SetTag("realm", context.Realm);
                    _authorizationCodeGrantTypeValidator.Validate(context);
                    var oauthClient = await AuthenticateClient(context, cancellationToken);
                    context.SetClient(oauthClient);
                    activity?.SetTag("client_id", oauthClient.ClientId);
                    await _dpopProofValidator.Validate(context);
                    var code = context.Request.RequestData.GetAuthorizationCode();
                    var redirectUri = context.Request.RequestData.GetRedirectUri();
                    var authCode = await _grantedTokenHelper.GetAuthorizationCode(code, cancellationToken);
                    var previousRequest = authCode?.OriginalRequest;
                    if (previousRequest == null)
                    {
                        // https://tools.ietf.org/html/rfc6749#section-4.1.2
                        var searchResult = await _grantedTokenHelper.GetTokensByAuthorizationCode(code, cancellationToken);
                        if (searchResult.Any())
                        {
                            await _grantedTokenHelper.RemoveTokens(searchResult, cancellationToken);
                            _logger.LogError($"authorization code '{code}' has already been used, all tokens previously issued have been revoked");
                            return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_GRANT, ErrorMessages.AUTHORIZATION_CODE_ALREADY_USED);
                        }

                        return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_GRANT, ErrorMessages.BAD_AUTHORIZATION_CODE);
                    }

                    CheckDPOPJkt(context, authCode);
                    var previousClientId = previousRequest.GetClientId();
                    var previousRedirectUrl = previousRequest.GetRedirectUri();
                    var claims = previousRequest.GetClaimsFromAuthorizationRequest();
                    if (!previousClientId.Equals(oauthClient.ClientId, StringComparison.InvariantCultureIgnoreCase)) return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_GRANT, ErrorMessages.AUTHORIZATION_CODE_NOT_ISSUED_BY_CLIENT);
                    if (!previousRedirectUrl.Equals(redirectUri, StringComparison.InvariantCultureIgnoreCase)) return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_GRANT, ErrorMessages.NOT_SAME_REDIRECT_URI);
                    await _grantedTokenHelper.RemoveAuthorizationCode(code, cancellationToken);

                    var scopes = GetScopes(previousRequest, context);
                    var resources = GetResources(previousRequest, context);
                    var authDetails = previousRequest.GetAuthorizationDetailsFromAuthorizationRequest();
                    var extractionResult = await _audienceHelper.Extract(context.Realm ?? Constants.DefaultRealm, scopes, resources, new List<string>(), authDetails, cancellationToken);
                    scopeLst = extractionResult.Scopes;
                    activity?.SetTag("scopes", string.Join(",", extractionResult.Scopes)); 
                    var result = BuildResult(context, extractionResult.Scopes);
                    await Authenticate(previousRequest, context, cancellationToken);
                    context.SetOriginalRequest(previousRequest);
                    var parameters = new BuildTokenParameter { AuthorizationDetails = extractionResult.AuthorizationDetails, Scopes = extractionResult.Scopes, Audiences = extractionResult.Audiences, Claims = claims, GrantId = authCode.GrantId };
                    foreach (var tokenBuilder in _tokenBuilders)
                        await tokenBuilder.Build(parameters, context, cancellationToken, true);

                    AddTokenProfile(context);
                    foreach (var kvp in context.Response.Parameters)
                        result.Add(kvp.Key, kvp.Value);

                    if (!string.IsNullOrWhiteSpace(authCode.GrantId))
                        result.Add(TokenResponseParameters.GrantId, authCode.GrantId);

                    await Enrich(context, result, cancellationToken);
                    await _busControl.Publish(new TokenIssuedSuccessEvent
                    {
                        GrantType = GRANT_TYPE,
                        ClientId = context.Client.ClientId,
                        Scopes = extractionResult.Scopes,
                        Realm = context.Realm
                    });
                    activity?.SetStatus(ActivityStatusCode.Ok, "Token has been issued");
                    return new OkObjectResult(result);
                }
                catch (OAuthUnauthorizedException ex)
                {
                    await _busControl.Publish(new TokenIssuedFailureEvent
                    {
                        GrantType = GRANT_TYPE,
                        ClientId = context.Client?.ClientId,
                        Scopes = scopeLst,
                        Realm = context.Realm,
                        ErrorMessage = ex.Message
                    });
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    return BuildError(HttpStatusCode.Unauthorized, ex.Code, ex.Message);
                }
                catch (OAuthDPoPRequiredException ex)
                {
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    context.Response.Response.Headers.Add(Constants.DPOPNonceHeaderName, ex.Nonce);
                    return BuildError(HttpStatusCode.Unauthorized, ex.Code, ex.Message);
                }
                catch (OAuthException ex)
                {
                    await _busControl.Publish(new TokenIssuedFailureEvent
                    {
                        GrantType = GRANT_TYPE,
                        ClientId = context.Client?.ClientId,
                        Scopes = scopeLst,
                        Realm = context.Realm,
                        ErrorMessage = ex.Message
                    });
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    return BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
                }
            }
        }

        protected virtual Task Enrich(HandlerContext handlerContext, JsonObject result, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        async Task Authenticate(JsonObject previousQueryParameters, HandlerContext handlerContext, CancellationToken token)
        {
            if (!previousQueryParameters.ContainsKey(JwtRegisteredClaimNames.Sub))
                return;
            var user = await _userRepository.Query().AsNoTracking().Include(u => u.Groups).Include(u => u.Realms).FirstOrDefaultAsync(u => u.Name == previousQueryParameters[JwtRegisteredClaimNames.Sub].GetValue<string>() && u.Realms.Any(r => r.RealmsName == handlerContext.Realm), token);
            var userClaims = await _userClaimsService.Get(user.Id, handlerContext.Realm, token);
            handlerContext.SetUser(user, userClaims);
        }

        void CheckDPOPJkt(HandlerContext context, AuthCode authCode)
        {
            if (context.DPOPProof == null || string.IsNullOrWhiteSpace(authCode.DPOPJkt)) return;
            if (context.DPOPProof.PublicKey().CreateThumbprint() != authCode.DPOPJkt) throw new OAuthException(ErrorCodes.INVALID_DPOP_PROOF, ErrorMessages.DPOP_JKT_MISMATCH);
        }
    }
}