// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Management.Handlers
{
    public interface IDeleteOAuthClientHandler
    {
        Task<bool> Handle(string clientId, CancellationToken cancellationToken);
    }

    public class DeleteOAuthClientHandler : IDeleteOAuthClientHandler
    {
        private readonly IOAuthClientRepository _oauthClientRepository;
        private readonly ILogger<DeleteOAuthClientHandler> _logger;

        public DeleteOAuthClientHandler(
            IOAuthClientRepository oauthClientRepository,
            ILogger<DeleteOAuthClientHandler> logger)
        {
            _oauthClientRepository = oauthClientRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(string clientId, CancellationToken cancellationToken)
        {
            var client = await _oauthClientRepository.FindOAuthClientById(clientId, cancellationToken);
            if (client == null)
            {
                _logger.LogError($"Client cannot be deleted because the client '{clientId}' doesn't exist");
                throw new OAuthClientNotFoundException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_CLIENT, clientId));
            }

            await _oauthClientRepository.Delete(client, cancellationToken);
            await _oauthClientRepository.SaveChanges(cancellationToken);
            _logger.LogInformation($"The client '{clientId}' has been removed");
            return true;
        }
    }
}
