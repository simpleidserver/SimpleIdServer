// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.DPoP;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Api.Token.TokenBuilders;
using SimpleIdServer.IdServer.Api.Token.TokenProfiles;
using SimpleIdServer.IdServer.Api.Token.Validators;
using SimpleIdServer.IdServer.Authenticate;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.IntegrationEvents;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.Handlers;

public class AuthorizationCodeHandler : BaseCredentialsHandler
{
    private readonly IGrantedTokenHelper _grantedTokenHelper;
    private readonly IAuthorizationCodeGrantTypeValidator _authorizationCodeGrantTypeValidator;
    private readonly IEnumerable<ITokenBuilder> _tokenBuilders;
    private readonly IUserRepository _userRepository;
    private readonly IUserSessionResitory _userSessionRepository;
    private readonly IGrantHelper _audienceHelper;
    private readonly IBusControl _busControl;
    private readonly IDPOPProofValidator _dpopProofValidator;
    private readonly IClientHelper _clientHelper;
    private readonly IPkceVerifier _pkceVerifier;
    private readonly ILogger<AuthorizationCodeHandler> _logger;

    public AuthorizationCodeHandler(
        IGrantedTokenHelper grantedTokenHelper,
        IAuthorizationCodeGrantTypeValidator authorizationCodeGrantTypeValidator, 
        IEnumerable<ITokenBuilder> tokenBuilders,
        IUserRepository usrRepository,
        IUserSessionResitory userSessionRepository,
        IClientAuthenticationHelper clientAuthenticationHelper,
        IGrantHelper audienceHelper,
        IBusControl busControl,
        IDPOPProofValidator dpopProofValidator,
        IOptions<IdServerHostOptions> options,
        IEnumerable<ITokenProfile> tokenProfiles,
        IClientHelper clientHelper,
        IPkceVerifier pkceVerifier,
        ILogger<AuthorizationCodeHandler> logger) : base(clientAuthenticationHelper, tokenProfiles, options)
    {
        _grantedTokenHelper = grantedTokenHelper;
        _authorizationCodeGrantTypeValidator = authorizationCodeGrantTypeValidator;
        _tokenBuilders = tokenBuilders;
        _userRepository = usrRepository;
        _userSessionRepository = userSessionRepository;
        _audienceHelper = audienceHelper;
        _busControl = busControl;
        _dpopProofValidator = dpopProofValidator;
        _clientHelper = clientHelper;
        _pkceVerifier = pkceVerifier;
        _logger = logger;
    }

    public override string GrantType => GRANT_TYPE;
    public const string GRANT_TYPE = "authorization_code";

    public override async Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken)
    {
        IEnumerable<string> scopeLst = new string[0];
        using (var activity = Tracing.BasicActivitySource.StartActivity("AuthorizationCodeHandler"))
        {
            try
            {
                activity?.SetTag(Tracing.IdserverTagNames.GrantType, GRANT_TYPE);
                activity?.SetTag(Tracing.CommonTagNames.Realm, context.Realm);
                _authorizationCodeGrantTypeValidator.Validate(context);
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
                        return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_GRANT, Global.AuthorizationCodeAlreadyUsed);
                    }

                    return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_GRANT, Global.BadAuthorizationCode);
                }

                await AuthenticateClient(context, authCode, cancellationToken); 
                if (!context.Client.IsSelfIssueEnabled && string.IsNullOrWhiteSpace(context.Request.RequestData.GetStr(TokenRequestParameters.RedirectUri))) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, TokenRequestParameters.RedirectUri));
                CheckDPOPJkt(context, authCode);
                var previousClientId = previousRequest.GetClientId();
                var previousRedirectUrl = previousRequest.GetRedirectUri();
                var claims = previousRequest.GetClaimsFromAuthorizationRequest();
                if (!previousClientId.Equals(context.Client.ClientId, StringComparison.InvariantCultureIgnoreCase)) return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_GRANT, Global.AuthorizationCodeNotIssuedByClient);
                if (!context.Client.IsSelfIssueEnabled && !previousRedirectUrl.Equals(redirectUri, StringComparison.InvariantCultureIgnoreCase)) return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_GRANT, Global.NotSameRedirectUri);
                await _grantedTokenHelper.RemoveAuthorizationCode(code, cancellationToken);

                var scopes = GetScopes(previousRequest, context);
                var resources = GetResources(previousRequest, context);
                var authDetails = previousRequest.GetAuthorizationDetailsFromAuthorizationRequest();
                var extractionResult = await _audienceHelper.Extract(context.Realm ?? Constants.DefaultRealm, scopes, resources, new List<string>(), authDetails, cancellationToken);
                scopeLst = extractionResult.Scopes;
                var result = BuildResult(context, extractionResult.Scopes);
                if(!context.Client.IsSelfIssueEnabled) await Authenticate(previousRequest, context, authCode, cancellationToken);
                else
                    context.SetUser(new User
                    {
                        Name = context.Client.ClientId
                    }, null);

                context.SetOriginalRequest(previousRequest);
                var additionalClaims = new Dictionary<string, object>();
                var parameters = new BuildTokenParameter { AuthorizationDetails = extractionResult.AuthorizationDetails, Scopes = extractionResult.Scopes, Audiences = extractionResult.Audiences, Claims = claims, GrantId = authCode.GrantId, AdditionalClaims = additionalClaims };
                foreach (var tokenBuilder in _tokenBuilders)
                {
                    if (tokenBuilder.Name == TokenResponseParameters.RefreshToken && !extractionResult.Scopes.Contains(Config.DefaultScopes.OfflineAccessScope.Name)) continue;
                    await tokenBuilder.Build(parameters, context, cancellationToken, true);
                }

                AddTokenProfile(context);
                foreach (var kvp in context.Response.Parameters)
                    result.Add(kvp.Key, kvp.Value);

                if (!string.IsNullOrWhiteSpace(authCode.GrantId))
                    result.Add(TokenResponseParameters.GrantId, authCode.GrantId);                        

                await Enrich(context, result, cancellationToken);
                Issue(result, context.Client.ClientId, context.Realm);
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
                Counters.FailToken(context.Client?.ClientId, context.Realm, GrantType);
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
                Counters.FailToken(context.Client?.ClientId, context.Realm, GrantType);
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                return BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }
    }

    protected async Task AuthenticateClient(HandlerContext context, AuthCode authCode, CancellationToken cancellationToken)
    {
        var clientId = context.Request.RequestData.GetClientId();
        if(_clientHelper.IsNonPreRegisteredRelyingParty(clientId))
        {
            var client = await _clientHelper.ResolveSelfDeclaredClient(authCode.OriginalRequest, cancellationToken);
            context.SetClient(client);
            return;
        }

        var oauthClient = await _clientHelper.ResolveClient(context.Realm, clientId, cancellationToken);
        if(!oauthClient.IsPublic) await Authenticate(context, oauthClient, cancellationToken);
        else await _pkceVerifier.Validate(context, oauthClient, context.GetIssuer(), cancellationToken);
        context.SetClient(oauthClient);
        await _dpopProofValidator.Validate(context);
    }

    protected virtual Task Enrich(HandlerContext handlerContext, JsonObject result, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    async Task Authenticate(JsonObject previousQueryParameters, HandlerContext handlerContext, AuthCode authCode, CancellationToken token)
    {
        if (!previousQueryParameters.ContainsKey(JwtRegisteredClaimNames.Sub))
            return;
        var user = await _userRepository.GetBySubject(previousQueryParameters[JwtRegisteredClaimNames.Sub].GetValue<string>(), handlerContext.Realm, token);
        UserSession session = null;
        if(!string.IsNullOrWhiteSpace(authCode.SessionId))
        {
            session = await _userSessionRepository.GetById(authCode.SessionId, handlerContext.Realm, token);
            if (session != null && !session.IsActive()) session = null;
        }

        handlerContext.SetUser(user, session);
    }

    void CheckDPOPJkt(HandlerContext context, AuthCode authCode)
    {
        if (context.DPOPProof == null || string.IsNullOrWhiteSpace(authCode.DPOPJkt)) return;
        if (context.DPOPProof.PublicKey().CreateThumbprint() != authCode.DPOPJkt) throw new OAuthException(ErrorCodes.INVALID_DPOP_PROOF, Global.DpopJktMismatch);
    }
}