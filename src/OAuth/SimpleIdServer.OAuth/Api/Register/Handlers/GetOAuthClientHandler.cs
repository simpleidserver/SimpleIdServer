// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Persistence;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Register.Handlers
{
    public interface IGetOAuthClientHandler
    {
        Task<JObject> Handle(string clientId, HandlerContext handlerContext, CancellationToken cancellationToken);
    }

    public class GetOAuthClientHandler : IGetOAuthClientHandler
    {
        private readonly IOAuthClientQueryRepository _oauthClientQueryRepository;
        private readonly IOAuthClientCommandRepository _oAuthClientCommandRepository;
        private ILogger<GetOAuthClientHandler> _logger;

        public GetOAuthClientHandler(
            IOAuthClientQueryRepository oauthClientQueryRepository,
            IOAuthClientCommandRepository oAuthClientCommandRepository,
            ILogger<GetOAuthClientHandler> logger)
        {
            _oauthClientQueryRepository = oauthClientQueryRepository;
            _oAuthClientCommandRepository = oAuthClientCommandRepository;
            _logger = logger;
        }

        public virtual async Task<JObject> Handle(string clientId, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            var accessToken = handlerContext.Request.GetToken(AutenticationSchemes.Bearer, AutenticationSchemes.Basic);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                _logger.LogError("access token is missing");
                throw new OAuthUnauthorizedException(ErrorCodes.INVALID_TOKEN, ErrorMessages.MISSING_ACCESS_TOKEN);
            }

            var clients = await _oauthClientQueryRepository.Find(new Persistence.Parameters.SearchClientParameter
            {
                RegistrationAccessToken = accessToken
            }, cancellationToken);
            if (!clients.Content.Any())
            {
                _logger.LogError($"access token '{accessToken}' is invalid");
                throw new OAuthUnauthorizedException(ErrorCodes.INVALID_TOKEN, ErrorMessages.BAD_ACCESS_TOKEN);
            }

            var client = clients.Content.First();
            if (client.ClientId != clientId)
            {
                client.RegistrationAccessToken = null;
                await _oAuthClientCommandRepository.Update(client, cancellationToken);
                await _oAuthClientCommandRepository.SaveChanges(cancellationToken);
                _logger.LogError($"access token '{accessToken}' can be used for the client '{client.ClientId}' and not for the client '{clientId}'");
                throw new OAuthUnauthorizedException(ErrorCodes.INVALID_TOKEN, string.Format(ErrorMessages.ACCESS_TOKEN_VALID_CLIENT, client.ClientId, clientId));
            }

            return BuildResponse(client, handlerContext.Request.IssuerName);
        }

        protected virtual JObject BuildResponse(OAuthClient oauthClient, string issuer)
        {
            return oauthClient.ToDto(issuer);
        }
    }
}
