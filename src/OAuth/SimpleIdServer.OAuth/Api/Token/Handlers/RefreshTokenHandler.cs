// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleIdServer.OAuth.Api.Token.Helpers;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.Api.Token.TokenProfiles;
using SimpleIdServer.OAuth.Api.Token.Validators;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Helpers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Token.Handlers
{
    public class RefreshTokenHandler : BaseCredentialsHandler
    {
        private readonly IRefreshTokenGrantTypeValidator _refreshTokenGrantTypeValidator;
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IEnumerable<ITokenProfile> _tokenProfiles;
        private readonly IEnumerable<ITokenBuilder> _tokenBuilders;
        private readonly ILogger<RefreshTokenHandler> _logger;

        public RefreshTokenHandler(
            IRefreshTokenGrantTypeValidator refreshTokenGrantTypeValidator, 
            IGrantedTokenHelper grantedTokenHelper, 
            IEnumerable<ITokenProfile> tokenProfiles,
            IEnumerable<ITokenBuilder> tokenBuilders, 
            IClientAuthenticationHelper clientAuthenticationHelper,
            ILogger<RefreshTokenHandler> logger) : base(clientAuthenticationHelper)
        {
            _refreshTokenGrantTypeValidator = refreshTokenGrantTypeValidator;
            _grantedTokenHelper = grantedTokenHelper;
            _tokenProfiles = tokenProfiles;
            _tokenBuilders = tokenBuilders;
            _logger = logger;
        }

        public const string GRANT_TYPE = "refresh_token";
        public override string GrantType { get => GRANT_TYPE; }

        public override async Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken)
        {
            try
            {
                _refreshTokenGrantTypeValidator.Validate(context);
                var oauthClient = await AuthenticateClient(context, cancellationToken);
                context.SetClient(oauthClient);
                var refreshToken = context.Request.RequestData.GetRefreshToken();
                var tokenResult = await _grantedTokenHelper.GetRefreshToken(refreshToken, cancellationToken);
                if (tokenResult == null)
                {
                    _logger.LogError($"refresh token '{refreshToken}' is invalid");
                    return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.BAD_REFRESH_TOKEN);
                }

                if (tokenResult.ExpirationTime != null && DateTime.UtcNow > tokenResult.ExpirationTime.Value)
                {
                    _logger.LogError($"refresh token '{refreshToken}' is expired");
                    await _grantedTokenHelper.RemoveRefreshToken(refreshToken, cancellationToken);
                    return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.REFRESH_TOKEN_IS_EXPIRED);
                }

                var jwsPayload = JsonObject.Parse(tokenResult.Data).AsObject();
                var clientId = jwsPayload.GetClientIdFromAuthorizationRequest();
                if (string.IsNullOrWhiteSpace(clientId))
                {
                    var clientAssertion = jwsPayload.GetClientAssertion();
                    if (string.IsNullOrWhiteSpace(clientAssertion))
                    {
                        _logger.LogError("client identifier cannot be extracted from the initial request");
                        return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.CLIENT_ID_CANNOT_BE_EXTRACTED);
                    }

                    var jwsHandler = new JwtSecurityTokenHandler();
                    var isJwsToken = jwsHandler.CanReadToken(clientAssertion);
                    if (!isJwsToken)
                    {
                        _logger.LogError("client_assertion doesn't have a correct JWS format");
                        throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.BAD_CLIENT_ASSERTION_FORMAT);
                    }

                    var extractedJws = jwsHandler.ReadJwtToken(clientAssertion);
                    if (extractedJws == null)
                    {
                        _logger.LogError("payload cannot be extracted from client_assertion");
                        throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.BAD_CLIENT_ASSERTION_FORMAT);
                    }

                    clientId = extractedJws.Issuer;
                }

                if (!clientId.Equals(oauthClient.ClientId, StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.LogError("refresh token is not issued by the client");
                    return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_GRANT, ErrorMessages.REFRESH_TOKEN_NOT_ISSUED_BY_CLIENT);
                }

                await _grantedTokenHelper.RemoveRefreshToken(refreshToken, cancellationToken);
                var scopes = jwsPayload.GetScopesFromAuthorizationRequest();
                var result = BuildResult(context, scopes);
                foreach (var tokenBuilder in _tokenBuilders)
                {
                    await tokenBuilder.Refresh(jwsPayload, context, cancellationToken);
                }
                
                _tokenProfiles.First(t => t.Profile == context.Client.PreferredTokenProfile).Enrich(context);
                foreach (var kvp in context.Response.Parameters)
                {
                    result.Add(kvp.Key, kvp.Value);
                }

                return new OkObjectResult(result);
            }
            catch (OAuthUnauthorizedException ex)
            {
                return BuildError(HttpStatusCode.Unauthorized, ex.Code, ex.Message);
            }
            catch (OAuthException ex)
            {
                return BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }
    }
}