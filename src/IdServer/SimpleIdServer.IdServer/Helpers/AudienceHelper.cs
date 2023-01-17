// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Store;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Helpers
{
    public interface IAudienceHelper
    {
        Task<AudienceResult> Extract(string clientId, IEnumerable<string> scopes, IEnumerable<string> resources, CancellationToken cancellationToken);
    }

    public class AudienceResult
    {
        public IEnumerable<string> Scopes { get; set; }
        public IEnumerable<string> Audiences { get; set; }
    }

    public class AudienceHelper : IAudienceHelper
    {
        private readonly IApiResourceRepository _apiResourceRepository;
        private readonly IClientRepository _clientRepository;

        public AudienceHelper(IApiResourceRepository apiResourceRepository, IClientRepository clientRepository)
        {
            _apiResourceRepository = apiResourceRepository;
            _clientRepository = clientRepository;
        }

        public async Task<AudienceResult> Extract(string clientId, IEnumerable<string> scopes, IEnumerable<string> resources, CancellationToken cancellationToken)
        {
            if (resources.Any())
                return await ProcessResourceParameter(resources, scopes, cancellationToken);
             return await ProcessScopeParameter(clientId, scopes, cancellationToken);


            async Task<AudienceResult> ProcessResourceParameter(IEnumerable<string> resources, IEnumerable<string> scopes, CancellationToken cancellationToken)
            {
                var apiResources = await _apiResourceRepository.Query().Include(r => r.Scopes).Where(r => resources.Contains(r.Name)).ToListAsync(cancellationToken);
                var unsupportedResources = resources.Where(r => !apiResources.Any(a => a.Name == r));
                if (unsupportedResources.Any())
                    throw new OAuthException(ErrorCodes.INVALID_TARGET, string.Format(ErrorMessages.UKNOWN_RESOURCE, string.Join(",", unsupportedResources)));
                var allApiResourceScopes = apiResources.SelectMany(c => c.Scopes).GroupBy(s => s.Name).Select(k => k.Key);
                var unsupportedScopes = scopes.Where(s => !allApiResourceScopes.Contains(s));
                if (unsupportedScopes.Any())
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(string.Format(ErrorMessages.UNSUPPORTED_SCOPES, string.Join(",", unsupportedScopes))));
                if (!scopes.Any())
                    scopes = allApiResourceScopes;
                return new AudienceResult { Audiences = resources, Scopes = scopes };
            }

            async Task<AudienceResult> ProcessScopeParameter(string clientId, IEnumerable<string> scopes, CancellationToken cancellationToken)
            {
                var audiences = await _clientRepository.Query().Include(c => c.Scopes).Where(c => c.Scopes.Any(s => scopes.Contains(s.Name))).Select(c => c.ClientId).ToListAsync(cancellationToken);
                if (!audiences.Contains(clientId)) audiences.Add(clientId);
                return new AudienceResult { Audiences = audiences, Scopes = scopes };
            }
        }
    }
}
