// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OAuth.Persistence.Parameters;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Management.Handlers
{
    public interface ISearchOAuthScopesHandler
    {
        Task<JObject> Handle(SearchScopeParameter parameter, CancellationToken cancellationToken);
    }

    public class SearchOAuthScopesHandler : ISearchOAuthScopesHandler
    {
        private readonly IOAuthScopeRepository _oauthScopeRepository;

        public SearchOAuthScopesHandler(IOAuthScopeRepository oauthScopeRepository)
        {
            _oauthScopeRepository = oauthScopeRepository;
        }

        public async Task<JObject> Handle(SearchScopeParameter parameter, CancellationToken cancellationToken)
        {
            var result = await _oauthScopeRepository.Find(parameter, cancellationToken);
            return new JObject
            {
                { "start_index", result.StartIndex },
                { "count", result.Count },
                { "total_length", result.TotalLength },
                { "content", new JArray(result.Content.Select(c => c.ToDto())) }
            };
        }
    }
}
