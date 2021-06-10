// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Management.Handlers
{
    public interface IUpdateOAuthScopeHandler
    {
        Task<bool> Handle(string scopeName, JObject jObj, CancellationToken cancellationToken);
    }

    public class UpdateOAuthScopeHandler : IUpdateOAuthScopeHandler
    {
        private readonly ILogger<UpdateOAuthScopeHandler> _logger;
        private readonly IOAuthScopeRepository _oauthScopeRepository;

        public UpdateOAuthScopeHandler(
            IOAuthScopeRepository oauthScopeRepository,
            ILogger<UpdateOAuthScopeHandler> logger)
        {
            _oauthScopeRepository = oauthScopeRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(string scopeName, JObject jObj, CancellationToken cancellationToken)
        {
            var scope = await _oauthScopeRepository.GetOAuthScope(scopeName, cancellationToken);
            if (scope == null)
            {
                _logger.LogError($"the scope '{scopeName}' doesn't exist");
                throw new OAuthScopeNotFoundException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_CLIENT, scopeName));
            }

            var extractedScope = jObj.ToScopeDomain();
            scope.SetClaims(extractedScope.Claims);
            await _oauthScopeRepository.Update(scope, cancellationToken);
            await _oauthScopeRepository.SaveChanges(cancellationToken);
            _logger.LogInformation($"the scope '{scopeName}' has been updated");
            return true;
        }
    }
}
