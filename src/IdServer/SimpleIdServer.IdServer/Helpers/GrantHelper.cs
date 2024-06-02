// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Helpers
{
    public interface IGrantHelper
    {
        Task<GrantRequest> Extract(string realm, IEnumerable<string> scopes, IEnumerable<string> resources, IEnumerable<string> audiences, ICollection<AuthorizationData> authorizationDetails, CancellationToken cancellationToken);
    }

    public class GrantRequest
    {
        public GrantRequest(ICollection<AuthorizedScope> authorizations, ICollection<AuthorizationData> authorizationDetails)
        {
            Authorizations = authorizations;
            AuthorizationDetails = authorizationDetails;
        }

        public IEnumerable<string> Scopes
        {
            get
            {
                return Authorizations.Select(a => a.Scope).Distinct().ToList();
            }
        }

        public IEnumerable<string> Audiences
        {
            get
            {
                return Authorizations.SelectMany(a => a.Audiences).Distinct().ToList();
            }
        }

        public ICollection<AuthorizedScope> Authorizations { get; private set; }
        public ICollection<AuthorizationData> AuthorizationDetails { get; private set; }
    }

    public class GrantHelper : IGrantHelper
    {
        private readonly IApiResourceRepository _apiResourceRepository;

        public GrantHelper(IApiResourceRepository apiResourceRepository)
        {
            _apiResourceRepository = apiResourceRepository;
        }

        public async Task<GrantRequest> Extract(string realm, IEnumerable<string> scopes, IEnumerable<string> resources, IEnumerable<string> audiences, ICollection<AuthorizationData> authorizationDetails, CancellationToken cancellationToken)
        {
            var authResults = new List<AuthorizedScope>();
            if (resources.Any())
                authResults.AddRange(await ProcessResourceParameter(resources, audiences, scopes, cancellationToken));

            var unknownScopes = scopes.Where(s => !authResults.Any(a => a.Scope == s));
            authResults.AddRange(await ProcessScopeParameter(scopes, cancellationToken));
            var resLst = authResults.SelectMany(a => a.Resources).Distinct();
            if(resLst.Any())
                authorizationDetails = authorizationDetails.Where(d => d.Locations != null && d.Locations.Any(l => resLst.Contains(l))).ToList();

            return new GrantRequest(authResults, authorizationDetails);

            async Task<List<AuthorizedScope>> ProcessResourceParameter(IEnumerable<string> resources, IEnumerable<string> audiences, IEnumerable<string> scopes, CancellationToken cancellationToken)
            {
                var authResults = new List<AuthorizedScope>();
                var apiResources = await _apiResourceRepository.GetByNamesOrAudiences(realm, resources.ToList(), audiences.ToList(), cancellationToken);
                var unsupportedResources = resources.Where(r => !apiResources.Any(a => a.Name == r));
                if (unsupportedResources.Any())
                    throw new OAuthException(ErrorCodes.INVALID_TARGET, string.Format(Global.UnknownResource, string.Join(",", unsupportedResources)));
                var unsupportedAudiences = audiences.Where(r => !apiResources.Any(a => a.Audience == r));
                if (unsupportedAudiences.Any())
                    throw new OAuthException(ErrorCodes.INVALID_TARGET, string.Format(Global.UnknownAudience, string.Join(",", unsupportedAudiences)));
                var allApiResourceScopes = apiResources.SelectMany(c => c.Scopes).GroupBy(s => s.Name).Select(k => k.Key);
                var supportedScopes = scopes.Where(s => apiResources.Any(r => r.Scopes.Any(sc => sc.Name == s)));
                if (!supportedScopes.Any())
                    supportedScopes = allApiResourceScopes;
                var result = new List<AuthorizedScope>();
                foreach(var scope in supportedScopes)
                    result.Add(new AuthorizedScope
                    {
                        Scope = scope,
                        AuthorizedResources = apiResources.Where(r => r.Scopes.Any(s => s.Name == scope)).Select(r =>
                            new AuthorizedResource
                            {
                                Audience = r.Audience,
                                Resource = r.Name
                            }
                        ).ToList()
                    });

                return result;
            }

            async Task<List<AuthorizedScope>> ProcessScopeParameter(IEnumerable<string> scopes, CancellationToken cancellationToken)
            {
                var apiResources = await _apiResourceRepository.GetByScopes(scopes.ToList(), cancellationToken);
                var result = new List<AuthorizedScope>();
                foreach (var scope in scopes)
                    result.Add(new AuthorizedScope
                    {
                        Scope = scope,
                        AuthorizedResources = apiResources.Where(r => r.Scopes.Any(s => s.Name == scope)).Select(r =>
                            new AuthorizedResource
                            {
                                Audience = r.Audience,
                                Resource = r.Name
                            }
                        ).ToList()
                    });

                return result;
            }
        }
    }
}
