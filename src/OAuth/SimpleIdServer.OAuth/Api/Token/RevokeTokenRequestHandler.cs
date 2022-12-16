// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using SimpleIdServer.OAuth.Api.Token.Helpers;
using SimpleIdServer.OAuth.Api.Token.Validators;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Helpers;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Token
{
    public interface IRevokeTokenRequestHandler
    {
        Task Handle(JsonObject jObjHeader, JsonObject jObjBody, X509Certificate2 certificate, string issuerName, CancellationToken token);
    }

    public class RevokeTokenRequestHandler : IRevokeTokenRequestHandler
    {
        private readonly IRevokeTokenValidator _revokeTokenValidator;
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IClientAuthenticationHelper _clientAuthenticationHelper;
        private readonly ILogger<RevokeTokenRequestHandler> _logger;

        public RevokeTokenRequestHandler(
            IRevokeTokenValidator revokeTokenValidator, 
            IGrantedTokenHelper grantedTokenHelper, 
            IClientAuthenticationHelper clientAuthenticationHelper,
            ILogger<RevokeTokenRequestHandler> logger)
        {
            _revokeTokenValidator = revokeTokenValidator;
            _grantedTokenHelper = grantedTokenHelper;
            _clientAuthenticationHelper = clientAuthenticationHelper;
            _logger = logger;
        }

        public async Task Handle(JsonObject jObjHeader, JsonObject jObjBody, X509Certificate2 certificate, string issuerName, CancellationToken cancellationToken)
        {
            var token = jObjBody.GetStr(RevokeTokenRequestParameters.Token);
            var validationResult = _revokeTokenValidator.Validate(jObjBody);
            var oauthClient = await _clientAuthenticationHelper.AuthenticateClient(jObjBody, jObjBody, certificate, issuerName, cancellationToken);
            bool isAccessTokenRemoved = false, isRefreshTokenRemoved = false;
            if (validationResult.TokenTypeHint == TokenResponseParameters.AccessToken)
            {
                isAccessTokenRemoved = await _grantedTokenHelper.TryRemoveAccessToken(token, oauthClient.ClientId, cancellationToken);
            }
            else if (validationResult.TokenTypeHint == TokenResponseParameters.RefreshToken)
            {
                isRefreshTokenRemoved = await _grantedTokenHelper.TryRemoveRefreshToken(token, oauthClient.ClientId, cancellationToken);
            }
            else
            {
                isAccessTokenRemoved = await _grantedTokenHelper.TryRemoveAccessToken(token, oauthClient.ClientId, cancellationToken);
                if (!isAccessTokenRemoved)
                {
                    isRefreshTokenRemoved = await _grantedTokenHelper.TryRemoveRefreshToken(token, oauthClient.ClientId, cancellationToken);
                }
            }

            if (isRefreshTokenRemoved)
            {
                _logger.LogInformation($"refresh token '{token}' has been removed");
            }

            if (isAccessTokenRemoved)
            {
                _logger.LogInformation($"access token '{token}' has been removed");
            }
        }
    }
}