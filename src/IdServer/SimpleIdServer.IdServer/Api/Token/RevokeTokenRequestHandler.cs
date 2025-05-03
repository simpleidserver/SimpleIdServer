// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Api.Token.Validators;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.IntegrationEvents;
using SimpleIdServer.IdServer.Helpers;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token
{
    public interface IRevokeTokenRequestHandler
    {
        Task Handle(string realm, JsonObject jObjHeader, JsonObject jObjBody, X509Certificate2 certificate, string issuerName, CancellationToken token);
    }

    public class RevokeTokenRequestHandler : IRevokeTokenRequestHandler
    {
        private readonly IRevokeTokenValidator _revokeTokenValidator;
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IClientAuthenticationHelper _clientAuthenticationHelper;
        private readonly IBusControl _busControl;
        private readonly ILogger<RevokeTokenRequestHandler> _logger;

        public RevokeTokenRequestHandler(
            IRevokeTokenValidator revokeTokenValidator, 
            IGrantedTokenHelper grantedTokenHelper, 
            IClientAuthenticationHelper clientAuthenticationHelper,
            IBusControl busControl,
            ILogger<RevokeTokenRequestHandler> logger)
        {
            _revokeTokenValidator = revokeTokenValidator;
            _grantedTokenHelper = grantedTokenHelper;
            _clientAuthenticationHelper = clientAuthenticationHelper;
            _busControl = busControl;
            _logger = logger;
        }

        public async Task Handle(string realm, JsonObject jObjHeader, JsonObject jObjBody, X509Certificate2 certificate, string issuerName, CancellationToken cancellationToken)
        {
            string token = null;
            using (var activity = Tracing.TokenActivitySource.StartActivity("Revoke Token"))
            {
                try
                {
                    token = jObjBody.GetStr(RevokeTokenRequestParameters.Token);
                    var validationResult = _revokeTokenValidator.Validate(jObjBody);
                    activity?.SetTag("token", token);
                    var oauthClient = await _clientAuthenticationHelper.AuthenticateClient(realm, jObjBody, jObjBody, certificate, issuerName, cancellationToken);
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
                        activity?.SetTag("token_type", "refresh_token");
                        _logger.LogInformation($"refresh token '{token}' has been removed");
                    }

                    if (isAccessTokenRemoved)
                    {
                        activity?.SetTag("token_type", "access_token");
                        _logger.LogInformation($"access token '{token}' has been removed");
                    }

                    if (isRefreshTokenRemoved || isAccessTokenRemoved)
                    {
                        activity?.SetStatus(ActivityStatusCode.Ok, "Token has been revoked");
                        await _busControl.Publish(new TokenRevokedSuccessEvent
                        {
                            Token = token,
                            Realm = realm
                        });
                    }
                    else
                    {
                        await _busControl.Publish(new TokenRevokedFailureEvent
                        {
                            Token = token,
                            Realm = realm,
                            ErrorMessage = "Token has not been revoked"
                        });
                        activity?.SetStatus(ActivityStatusCode.Error, "Token has not been revoked");
                    }
                }
                catch(OAuthException ex)
                {
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    await _busControl.Publish(new TokenRevokedFailureEvent
                    {
                        Token = token,
                        Realm = realm,
                        ErrorMessage = ex.Message
                    });
                    throw ex;
                }
            }
        }
    }
}