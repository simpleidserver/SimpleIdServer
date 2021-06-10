// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Management.Handlers
{
    public interface IAddOAuthScopeHandler
    {
        Task<string> Handle(JObject jObj, CancellationToken cancellationToken);
    }

    public class AddOAuthScopeHandler : IAddOAuthScopeHandler
    {
        private readonly IOAuthScopeRepository _oauthScopeRepository;
        private readonly ILogger<AddOAuthScopeHandler> _logger;

        public AddOAuthScopeHandler(
            IOAuthScopeRepository oauthScopeRepository,
            ILogger<AddOAuthScopeHandler> logger)
        {
            _oauthScopeRepository = oauthScopeRepository;
            _logger = logger;
        }

        public async Task<string> Handle(JObject jObj, CancellationToken cancellationToken)
        {
            var extractedScope = jObj.ToScopeDomain();
            var result = await _oauthScopeRepository.GetOAuthScope(extractedScope.Name, cancellationToken);
            if (result != null)
            {
                _logger.LogError($"the scope '{extractedScope.Name}' already exists");
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.SCOPE_ALREADY_EXISTS, extractedScope.Name));
            }

            var scope = OAuthScope.Create(extractedScope.Name);
            await _oauthScopeRepository.Add(scope, cancellationToken);
            await _oauthScopeRepository.SaveChanges(cancellationToken);
            _logger.LogInformation($"the scope '{extractedScope.Name}' has been added");
            return extractedScope.Name;
        }
    }
}
