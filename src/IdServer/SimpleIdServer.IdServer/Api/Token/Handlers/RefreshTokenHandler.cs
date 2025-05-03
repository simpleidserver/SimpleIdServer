// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.DPoP;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Api.Token.TokenBuilders;
using SimpleIdServer.IdServer.Api.Token.TokenProfiles;
using SimpleIdServer.IdServer.Api.Token.Validators;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.IntegrationEvents;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.Handlers
{
    public class RefreshTokenHandler : BaseCredentialsHandler
    {
        private readonly IRefreshTokenGrantTypeValidator _refreshTokenGrantTypeValidator;
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IEnumerable<ITokenBuilder> _tokenBuilders;
        private readonly IUserRepository _userRepository;
        private readonly IUserSessionResitory _userSessionRepository;
        private readonly IGrantHelper _audienceHelper;
        private readonly IBusControl _busControl;
        private readonly IDPOPProofValidator _dpopProofValidator;
        private readonly ITransactionBuilder _transactionBuilder;
        private readonly ILogger<RefreshTokenHandler> _logger;

        public RefreshTokenHandler(
            IRefreshTokenGrantTypeValidator refreshTokenGrantTypeValidator, 
            IGrantedTokenHelper grantedTokenHelper,
            IEnumerable<ITokenProfile> tokenProfiles,
            IEnumerable<ITokenBuilder> tokenBuilders, 
            IUserRepository userRepository,
            IUserSessionResitory userSessionRepository,
            IClientAuthenticationHelper clientAuthenticationHelper,
            IGrantHelper audienceHelper,
            IBusControl busControl,
            IDPOPProofValidator dpopProofValidator,
            ITransactionBuilder transactionBuilder,
            IOptions<IdServerHostOptions> options,
            ILogger<RefreshTokenHandler> logger) : base(clientAuthenticationHelper, tokenProfiles, options)
        {
            _refreshTokenGrantTypeValidator = refreshTokenGrantTypeValidator;
            _grantedTokenHelper = grantedTokenHelper;
            _tokenBuilders = tokenBuilders;
            _userRepository = userRepository;
            _userSessionRepository = userSessionRepository;
            _busControl = busControl;
            _dpopProofValidator = dpopProofValidator;
            _audienceHelper = audienceHelper;
            _transactionBuilder = transactionBuilder;
            _logger = logger;
        }

        public const string GRANT_TYPE = "refresh_token";
        public override string GrantType { get => GRANT_TYPE; }

        public override async Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken)
        {
            IEnumerable<string> scopeLst = new string[0];
            using (var activity = Tracing.TokenActivitySource.StartActivity("Get Token"))
            {
                try
                {
                    activity?.SetTag("grant_type", GRANT_TYPE);
                    activity?.SetTag("realm", context.Realm);
                    _refreshTokenGrantTypeValidator.Validate(context);
                    var oauthClient = await AuthenticateClient(context, cancellationToken);
                    context.SetClient(oauthClient);
                    activity?.SetTag("client_id", oauthClient.ClientId);
                    await _dpopProofValidator.Validate(context);
                    var refreshToken = context.Request.RequestData.GetRefreshToken();
                    var tokenResult = await _grantedTokenHelper.GetRefreshToken(refreshToken, cancellationToken);
                    if (tokenResult == null)
                    {
                        _logger.LogError($"refresh token '{refreshToken}' is invalid");
                        return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.BadRefreshToken);
                    }

                    if (tokenResult.ExpirationTime != null && DateTime.UtcNow > tokenResult.ExpirationTime.Value)
                    {
                        _logger.LogError($"refresh token '{refreshToken}' is expired");
                        await _grantedTokenHelper.RemoveRefreshToken(refreshToken, cancellationToken);
                        return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.RefreshTokenIsExpired);
                    }

                    if(context.DPOPProof != null)
                    {
                        var receivedJkt = context.DPOPProof.PublicKey().CreateThumbprint();
                        if (tokenResult.Jkt != receivedJkt) throw new OAuthException(ErrorCodes.INVALID_DPOP_PROOF, Global.InvalidDpopBoundToAccessToken);
                    }

                    var jwsPayload = JsonObject.Parse(tokenResult.Data).AsObject();
                    var originalJwsPayload = tokenResult.OriginalData == null ? null : JsonObject.Parse(tokenResult.OriginalData).AsObject();
                    if(originalJwsPayload != null)
                        context.SetOriginalRequest(originalJwsPayload);
                    else
                        context.SetOriginalRequest(jwsPayload);

                    var clientId = jwsPayload.GetClientIdFromAuthorizationRequest();
                    if (string.IsNullOrWhiteSpace(clientId))
                    {
                        var clientAssertion = jwsPayload.GetClientAssertion();
                        if (string.IsNullOrWhiteSpace(clientAssertion))
                        {
                            _logger.LogError("client identifier cannot be extracted from the initial request");
                            return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.ClientIdCannotBeExtracted);
                        }

                        var jwsHandler = new JwtSecurityTokenHandler();
                        var isJwsToken = jwsHandler.CanReadToken(clientAssertion);
                        if (!isJwsToken)
                        {
                            _logger.LogError("client_assertion doesn't have a correct JWS format");
                            throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.BadClientAssertionFormat);
                        }

                        var extractedJws = jwsHandler.ReadJwtToken(clientAssertion);
                        if (extractedJws == null)
                        {
                            _logger.LogError("payload cannot be extracted from client_assertion");
                            throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.BadClientAssertionFormat);
                        }

                        clientId = extractedJws.Issuer;
                    }

                    if (!clientId.Equals(oauthClient.ClientId, StringComparison.InvariantCultureIgnoreCase))
                    {
                        _logger.LogError("refresh token is not issued by the client");
                        return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_GRANT, Global.RefreshTokenNotIssuedByClient);
                    }

                    await _grantedTokenHelper.RemoveRefreshToken(refreshToken, cancellationToken);
                    var scopes = GetScopes(originalJwsPayload, jwsPayload);
                    var resources = GetResources(originalJwsPayload, jwsPayload);
                    var claims = GetClaims(originalJwsPayload, jwsPayload);
                    var authDetails = GetAuthorizationDetails(originalJwsPayload, jwsPayload);
                    var extractionResult = await _audienceHelper.Extract(context.Realm ?? Constants.DefaultRealm, scopes, resources, new List<string>(), authDetails, cancellationToken);
                    scopeLst = extractionResult.Scopes;
                    activity?.SetTag("scopes", string.Join(",", extractionResult.Scopes));
                    var result = BuildResult(context, extractionResult.Scopes);
                    await Authenticate(jwsPayload, context, tokenResult, cancellationToken);
                    foreach (var tokenBuilder in _tokenBuilders)
                        await tokenBuilder.Build(new BuildTokenParameter { AuthorizationDetails = extractionResult.AuthorizationDetails, Scopes = extractionResult.Scopes, Audiences = extractionResult.Audiences, Claims = claims, GrantId = tokenResult.GrantId }, context, cancellationToken);

                    AddTokenProfile(context);
                    foreach (var kvp in context.Response.Parameters)
                        result.Add(kvp.Key, kvp.Value);

                    if (!string.IsNullOrWhiteSpace(tokenResult.GrantId))
                        result.Add(TokenResponseParameters.GrantId, tokenResult.GrantId);

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

        async Task Authenticate(JsonObject previousQueryParameters, HandlerContext handlerContext, Domains.Token tokenResult, CancellationToken token)
        {
            if (!previousQueryParameters.ContainsKey(JwtRegisteredClaimNames.Sub))
                return;
            var result = await _userRepository.GetBySubject(previousQueryParameters[JwtRegisteredClaimNames.Sub].GetValue<string>(), handlerContext.Realm ?? Constants.DefaultRealm, token);
            UserSession session = null;
            if(!string.IsNullOrWhiteSpace(tokenResult.SessionId))
            {
                using (var transaction = _transactionBuilder.Build())
                {
                    session = await _userSessionRepository.GetById(tokenResult.SessionId, handlerContext.Realm ?? Constants.DefaultRealm, token);
                    if (session != null && !session.IsActive()) session = null;
                    else
                    {
                        var currentDateTime = DateTime.UtcNow;
                        var expirationTimeInSeconds = GetExpirationTimeInSeconds(handlerContext.Client);
                        var expirationDateTime = currentDateTime.AddSeconds(expirationTimeInSeconds);
                        session.ExpirationDateTime = expirationDateTime;
                        _userSessionRepository.Update(session);
                        await transaction.Commit(token);
                    }
                }
            }

            handlerContext.SetUser(result, session);
        }

        double GetExpirationTimeInSeconds(Client client)
        {
            var expirationTimeInSeconds = client == null || client.TokenExpirationTimeInSeconds == null ?
               Options.DefaultTokenExpirationTimeInSeconds : client.TokenExpirationTimeInSeconds.Value;
            return expirationTimeInSeconds;
        }
    }
}