// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleIdServer.Domains;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.Store;
using System;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Register.Handlers
{
    public interface IUpdateOAuthClientHandler
    {
        Task<bool> Handle(string clientId, JsonObject content, CancellationToken cancellationToken);
    }

    public class UpdateOAuthClientHandler : IUpdateOAuthClientHandler
    {
        private readonly IClientRepository _clientRepository;
        private readonly IScopeRepository _scopeRepository;
        private readonly ILogger<UpdateOAuthClientHandler> _logger;

        public UpdateOAuthClientHandler(
            IClientRepository clientRepository,
            IScopeRepository scopeRepository,
            ILogger<UpdateOAuthClientHandler> logger)
        {
            _clientRepository = clientRepository;
            _scopeRepository = scopeRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(string clientId, JsonObject content, CancellationToken cancellationToken)
        {
            var extractedClient = ExtractClient(content);
            var oauthClient = await _clientRepository.Query().FirstOrDefaultAsync(c => c.ClientId == clientId, cancellationToken);
            if (oauthClient == null)
            {
                _logger.LogError($"The client '{clientId}' doesn't exist");
                throw new OAuthClientNotFoundException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_CLIENT, clientId));
            }

            await UpdateClient(oauthClient, extractedClient, cancellationToken);
            await _clientRepository.SaveChanges(cancellationToken);
            return true;
        }

        protected virtual async Task UpdateClient(Client oauthClient, Client extractClient, CancellationToken cancellationToken)
        {
            var scopeNames = extractClient.Scopes.Select(s => s.Name);
            var scopes = await _scopeRepository.Query().Where(s => scopeNames.Contains(s.Name));
            oauthClient.SetClientNames(extractClient.ClientNames);
            oauthClient.SetLogoUris(extractClient.LogoUris);
            oauthClient.TokenEndPointAuthMethod = extractClient.TokenEndPointAuthMethod;
            oauthClient.SetAllowedScopes(scopes);
            oauthClient.RedirectionUrls = extractClient.RedirectionUrls;
            oauthClient.TokenExpirationTimeInSeconds = extractClient.TokenExpirationTimeInSeconds;
            oauthClient.RefreshTokenExpirationTimeInSeconds = extractClient.RefreshTokenExpirationTimeInSeconds;
            oauthClient.GrantTypes = extractClient.GrantTypes;
            oauthClient.UpdateDateTime = DateTime.UtcNow;
        }

        protected virtual Client ExtractClient(JsonObject content)
        {
            var result = content.ToDomain();
            return result;
        }
    }
}
