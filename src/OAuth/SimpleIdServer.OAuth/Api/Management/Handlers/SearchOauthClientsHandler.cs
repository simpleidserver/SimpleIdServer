// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OAuth.Persistence.Parameters;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Management.Handlers
{
    public interface ISearchOauthClientsHandler
    {
        Task<JObject> Handle(SearchClientParameter parameter, string issuer, CancellationToken cancellationToken);
    }

    public class SearchOauthClientsHandler: ISearchOauthClientsHandler
    {
        private readonly IOAuthClientRepository _oauthClientRepository;

        public SearchOauthClientsHandler(IOAuthClientRepository oauthClientRepository)
        {
            _oauthClientRepository = oauthClientRepository;
        }

        public async Task<JObject> Handle(SearchClientParameter parameter, string issuer, CancellationToken cancellationToken)
        {
            var result = await _oauthClientRepository.Find(parameter, cancellationToken);
            return new JObject
            {
                { "start_index", result.StartIndex },
                { "count", result.Count },
                { "total_length", result.TotalLength },
                { "content", new JArray(result.Content.Select(c => ToDto(c, issuer))) }
            };
        }

        protected virtual JObject ToDto(BaseClient client, string issuer)
        {
            return (client as OAuthClient).ToDto(issuer);
        }
    }
}
