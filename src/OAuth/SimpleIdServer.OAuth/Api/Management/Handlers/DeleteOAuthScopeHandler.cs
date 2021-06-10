// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Management.Handlers
{
    public interface IDeleteOAuthScopeHandler
    {
        Task<bool> Handle(string scopeName, CancellationToken cancellationToken); 
    }

    public class DeleteOAuthScopeHandler : IDeleteOAuthScopeHandler
    {
        private readonly IOAuthScopeRepository _oauthScopeRepository;
        private readonly ILogger<DeleteOAuthScopeHandler> _logger;

        public DeleteOAuthScopeHandler(
            IOAuthScopeRepository oauthScopeRepository,
            ILogger<DeleteOAuthScopeHandler> logger)
        {
            _oauthScopeRepository = oauthScopeRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(string scopeName, CancellationToken cancellationToken)
        {
            var scope = await _oauthScopeRepository.GetOAuthScope(scopeName, cancellationToken);
            if (scope == null)
            {
                _logger.LogError($"the scope '{scopeName}' doesn't exist");
                throw new OAuthScopeNotFoundException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_CLIENT, scopeName));
            }

            await _oauthScopeRepository.Delete(scope, cancellationToken);
            await _oauthScopeRepository.SaveChanges(cancellationToken);
            _logger.LogError($"the scope '{scopeName}' is removed");
            return true;
        }
    }
}
