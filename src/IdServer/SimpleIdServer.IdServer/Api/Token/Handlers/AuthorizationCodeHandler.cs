// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
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
        private readonly IAuthorizationCodeGrantTypeValidator _authorizationCodeGrantTypeValidator;
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IEnumerable<ITokenProfile> _tokenProfiles;
        private readonly IEnumerable<ITokenBuilder> _tokenBuilders;
        private readonly IUserRepository _userRepository;
        private readonly IGrantHelper _audienceHelper;
        private readonly IdServerHostOptions _options;
        private readonly IBusControl _busControl;
        private readonly ILogger<AuthorizationCodeHandler> _logger;

        public AuthorizationCodeHandler(
            IAuthorizationCodeGrantTypeValidator authorizationCodeGrantTypeValidator, 
            IGrantedTokenHelper grantedTokenHelper, 
            IEnumerable<ITokenProfile> tokenProfiles,
            IEnumerable<ITokenBuilder> tokenBuilders,
            IUserRepository usrRepository,
            IClientAuthenticationHelper clientAuthenticationHelper,
            IGrantHelper audienceHelper,
            IBusControl busControl,
            IOptions<IdServerHostOptions> options,
            ILogger<AuthorizationCodeHandler> logger) : base(clientAuthenticationHelper, options)
        {
            _authorizationCodeGrantTypeValidator = authorizationCodeGrantTypeValidator;
            _grantedTokenHelper = grantedTokenHelper;
            _tokenProfiles = tokenProfiles;
            _tokenBuilders = tokenBuilders;
            _userRepository = usrRepository;
            _audienceHelper = audienceHelper;
            _options = options.Value;
            _busControl = busControl;
            _logger = logger;
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

                    var previousClientId = previousRequest.GetClientId();
                    var previousRedirectUrl = previousRequest.GetRedirectUri();
                    var claims = previousRequest.GetClaimsFromAuthorizationRequest();
                    if (!previousClientId.Equals(oauthClient.ClientId, StringComparison.InvariantCultureIgnoreCase)) return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_GRANT, ErrorMessages.AUTHORIZATION_CODE_NOT_ISSUED_BY_CLIENT);
                    if (!previousRedirectUrl.Equals(redirectUri, StringComparison.InvariantCultureIgnoreCase)) return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_GRANT, ErrorMessages.NOT_SAME_REDIRECT_URI);
                    await _grantedTokenHelper.RemoveAuthorizationCode(code, cancellationToken);

                    var scopes = GetScopes(previousRequest, context);
                    var resources = GetResources(previousRequest, context);
                    var authDetails = previousRequest.GetAuthorizationDetailsFromAuthorizationRequest();
                    var extractionResult = await _audienceHelper.Extract(context.Realm ?? Constants.DefaultRealm, scopes, resources, authDetails, cancellationToken);
                    scopeLst = extractionResult.Scopes;
                    activity?.SetTag("scopes", string.Join(",", extractionResult.Scopes)); 
                    var result = BuildResult(context, extractionResult.Scopes);
                    await Authenticate(previousRequest, context, cancellationToken);
                    context.SetOriginalRequest(previousRequest);

                    foreach (var tokenBuilder in _tokenBuilders)
                        await tokenBuilder.Build(new BuildTokenParameter { AuthorizationDetails = extractionResult.AuthorizationDetails, Scopes = extractionResult.Scopes, Audiences = extractionResult.Audiences, Claims = claims, GrantId = authCode.GrantId }, context, cancellationToken, true);

                    _tokenProfiles.First(t => t.Profile == (context.Client.PreferredTokenProfile ?? _options.DefaultTokenProfile)).Enrich(context);
                    foreach (var kvp in context.Response.Parameters)
                    {
                        result.Add(kvp.Key, kvp.Value);
                    }

                    if (!string.IsNullOrWhiteSpace(authCode.GrantId))
                        result.Add(TokenResponseParameters.GrantId, authCode.GrantId);

                    await _busControl.Publish(new TokenIssuedSuccessEvent
                    {
                        GrantType = GRANT_TYPE,
                        ClientId = context.Client.Id,
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
                        ClientId = context.Client?.Id,
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
                        ClientId = context.Client?.Id,
                        Scopes = scopeLst,
                        Realm = context.Realm,
                        ErrorMessage = ex.Message
                    });
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    return BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
                }
            }
        }

        async Task Authenticate(JsonObject previousQueryParameters, HandlerContext handlerContext, CancellationToken token)
        {
            if (!previousQueryParameters.ContainsKey(JwtRegisteredClaimNames.Sub))
                return;
            handlerContext.SetUser(await _userRepository.Query().AsNoTracking().Include(u => u.OAuthUserClaims).Include(u => u.Groups).Include(u => u.Realms).FirstOrDefaultAsync(u => u.Name == previousQueryParameters[JwtRegisteredClaimNames.Sub].GetValue<string>() && u.Realms.Any(r => r.RealmsName == handlerContext.Realm), token));
        }
    }
}