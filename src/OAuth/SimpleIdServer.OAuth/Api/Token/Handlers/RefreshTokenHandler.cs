// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api.Token.Helpers;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.Api.Token.TokenProfiles;
using SimpleIdServer.OAuth.Api.Token.Validators;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
                var refreshToken = context.Request.Data.GetRefreshToken();
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

                var jwsPayload = JObject.Parse(tokenResult.Data);
                if (!jwsPayload.GetClientIdFromAuthorizationRequest().Equals(oauthClient.ClientId, StringComparison.InvariantCultureIgnoreCase))
                {
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