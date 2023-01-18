// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Store;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Helpers
{
    public interface IGrantHelper
    {
        Task<GrantRequest> Extract(string clientId, IEnumerable<string> scopes, IEnumerable<string> resources, CancellationToken cancellationToken);
    }

    public class GrantRequest
    {
        public GrantRequest(ICollection<AuthorizedScope> authorizations, GrantRequestTypes type)
        {
            Authorizations = authorizations;
            Type = type;
        }

        public IEnumerable<string> Scopes
        {
            get
            {
                return Authorizations.Select(a => a.Scope).Distinct();
            }
        }

        public IEnumerable<string> Audiences
        {
            get
            {
                return Authorizations.SelectMany(a => a.Resources).Distinct();
            }
        }

        public IEnumerable<string> Resources
        {
            get
            {
                return Type == GrantRequestTypes.IDENTITY ? new List<string>() : Audiences;
            }
        }

        public ICollection<AuthorizedScope> Authorizations { get; private set; }
        public GrantRequestTypes Type { get; private set; }
    }

    public enum GrantRequestTypes
    {
        IDENTITY = 0,
        API = 1
    }

    public class GrantHelper : IGrantHelper
    {
        private readonly IApiResourceRepository _apiResourceRepository;
        private readonly IClientRepository _clientRepository;

        public GrantHelper(IApiResourceRepository apiResourceRepository, IClientRepository clientRepository)
        {
            _apiResourceRepository = apiResourceRepository;
            _clientRepository = clientRepository;
        }

        public async Task<GrantRequest> Extract(string clientId, IEnumerable<string> scopes, IEnumerable<string> resources, CancellationToken cancellationToken)
        {
            if (resources.Any())
                return await ProcessResourceParameter(resources, scopes, cancellationToken);
             return await ProcessScopeParameter(clientId, scopes, cancellationToken);


            async Task<GrantRequest> ProcessResourceParameter(IEnumerable<string> resources, IEnumerable<string> scopes, CancellationToken cancellationToken)
            {
                var authResults = new List<AuthorizedScope>();
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
                var result = new List<AuthorizedScope>();
                foreach(var scope in scopes)
                    result.Add(new AuthorizedScope
                    {
                        Scope = scope,
                        Resources = apiResources.Where(r => r.Scopes.Any(s => s.Name == scope)).Select(r => r.Name).ToList()
                    });

                return new GrantRequest(result, GrantRequestTypes.API);
            }

            async Task<GrantRequest> ProcessScopeParameter(string clientId, IEnumerable<string> scopes, CancellationToken cancellationToken)
            {
                var clients = await _clientRepository.Query().Include(c => c.Scopes).Where(c => c.Scopes.Any(s => scopes.Contains(s.Name))).ToListAsync(cancellationToken);
                var result = new List<AuthorizedScope>();
                foreach (var scope in scopes)
                    result.Add(new AuthorizedScope
                    {
                        Scope = scope,
                        Resources = clients.Where(c => c.Scope == scope).Select(c => c.ClientId).ToList()
                    });

                return new GrantRequest(result, GrantRequestTypes.IDENTITY);
            }
        }
    }
}
