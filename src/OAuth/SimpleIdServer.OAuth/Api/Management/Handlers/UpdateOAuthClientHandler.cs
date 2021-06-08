// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Persistence;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Management.Handlers
{
    public interface IUpdateOAuthClientHandler
    {
        Task<bool> Handle(string clientId, JObject content, CancellationToken cancellationToken);
    }

    public class UpdateOAuthClientHandler : IUpdateOAuthClientHandler
    {
        private readonly IOAuthClientRepository _oauthClientRepository;
        private readonly IOAuthScopeRepository _oauthScopeRepository;
        private readonly ILogger<UpdateOAuthClientHandler> _logger;

        public UpdateOAuthClientHandler(
            IOAuthClientRepository oauthClientRepository,
            IOAuthScopeRepository oauthScopeRepository,
            ILogger<UpdateOAuthClientHandler> logger)
        {
            _oauthClientRepository = oauthClientRepository;
            _oauthScopeRepository = oauthScopeRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(string clientId, JObject content, CancellationToken cancellationToken)
        {
            var extractedClient = ExtractClient(content);
            var oauthClient = await _oauthClientRepository.FindOAuthClientById(clientId, cancellationToken);
            if(oauthClient == null)
            {
                _logger.LogError($"The client '{clientId}' doesn't exist");
                throw new OAuthClientNotFoundException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_CLIENT, clientId));
            }

            await UpdateClient(oauthClient, extractedClient, cancellationToken);
            await _oauthClientRepository.Update(oauthClient, cancellationToken);
            await _oauthClientRepository.SaveChanges(cancellationToken);
            return true;
        }

        protected virtual async Task UpdateClient(BaseClient oauthClient, BaseClient extractClient, CancellationToken cancellationToken)
        {
            var scopeNames = extractClient.AllowedScopes.Select(s => s.Name);
            var scopes = await _oauthScopeRepository.FindOAuthScopesByNames(scopeNames, cancellationToken);
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

        protected virtual BaseClient ExtractClient(JObject content)
        {
            var result = content.ToDomain();
            return result;
        }
    }
}
