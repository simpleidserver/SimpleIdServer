﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Persistence;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Register.Handlers
{
    public class BaseOAuthClientHandler
    {
        public BaseOAuthClientHandler(
            IOAuthClientRepository oAuthClientRepository,
            ILogger<BaseOAuthClientHandler> logger)
        {
            OAuthClientRepository = oAuthClientRepository;
            Logger = logger;
        }


        public IOAuthClientRepository OAuthClientRepository { get; set; }
        public ILogger<BaseOAuthClientHandler> Logger { get; set; }

        public virtual async Task<BaseClient> GetClient(string clientId, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            var accessToken = handlerContext.Request.GetToken(AutenticationSchemes.Bearer, AutenticationSchemes.Basic);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                Logger.LogError("access token is missing");
                throw new OAuthUnauthorizedException(ErrorCodes.INVALID_TOKEN, ErrorMessages.MISSING_ACCESS_TOKEN);
            }

            var clients = await OAuthClientRepository.Find(new Persistence.Parameters.SearchClientParameter
            {
                RegistrationAccessToken = accessToken
            }, cancellationToken);
            if (!clients.Content.Any())
            {
                Logger.LogError($"access token '{accessToken}' is invalid");
                throw new OAuthUnauthorizedException(ErrorCodes.INVALID_TOKEN, ErrorMessages.BAD_ACCESS_TOKEN);
            }

            var client = clients.Content.First();
            if (client.ClientId != clientId)
            {
                client.RegistrationAccessToken = null;
                await OAuthClientRepository.Update(client, cancellationToken);
                await OAuthClientRepository.SaveChanges(cancellationToken);
                Logger.LogError($"access token '{accessToken}' can be used for the client '{client.ClientId}' and not for the client '{clientId}'");
                throw new OAuthUnauthorizedException(ErrorCodes.INVALID_TOKEN, string.Format(ErrorMessages.ACCESS_TOKEN_VALID_CLIENT, client.ClientId, clientId));
            }

            return client;
        }
    }
}
