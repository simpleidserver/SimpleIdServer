// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleIdServer.Domains;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.Store;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Register.Handlers
{
    public class BaseOAuthClientHandler
    {
        public BaseOAuthClientHandler(
            IClientRepository clientRepository,
            ILogger<BaseOAuthClientHandler> logger)
        {
            ClientRepository = clientRepository;
            Logger = logger;
        }


        public IClientRepository ClientRepository { get; set; }
        public ILogger<BaseOAuthClientHandler> Logger { get; set; }

        public virtual async Task<Client> GetClient(string clientId, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            var accessToken = handlerContext.Request.GetToken(AutenticationSchemes.Bearer, AutenticationSchemes.Basic);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                Logger.LogError("access token is missing");
                throw new OAuthUnauthorizedException(ErrorCodes.INVALID_TOKEN, ErrorMessages.MISSING_ACCESS_TOKEN);
            }

            var clients = await ClientRepository.Query().Where(c => c.RegistrationAccessToken == accessToken).ToListAsync(cancellationToken);
            if (!clients.Any())
            {
                Logger.LogError($"access token '{accessToken}' is invalid");
                throw new OAuthUnauthorizedException(ErrorCodes.INVALID_TOKEN, ErrorMessages.BAD_ACCESS_TOKEN);
            }

            var client = clients.First();
            if (client.ClientId != clientId)
            {
                client.RegistrationAccessToken = null;
                await ClientRepository.SaveChanges(cancellationToken);
                Logger.LogError($"access token '{accessToken}' can be used for the client '{client.ClientId}' and not for the client '{clientId}'");
                throw new OAuthUnauthorizedException(ErrorCodes.INVALID_TOKEN, string.Format(ErrorMessages.ACCESS_TOKEN_VALID_CLIENT, client.ClientId, clientId));
            }

            return client;
        }
    }
}
