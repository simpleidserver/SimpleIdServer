// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Saml.Idp.Extensions;
using SimpleIdServer.Saml.Idp.Persistence;
using SimpleIdServer.Saml.Idp.Persistence.Parameters;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Idp.Apis.RelyingParties.Handlers
{
    public interface ISearchRelyingPartiesHandler
    {
        Task<JObject> Handle(SearchRelyingPartiesParameter parameter, CancellationToken cancellationToken);
    }

    public class SearchRelyingPartiesHandler : ISearchRelyingPartiesHandler
    {
        private readonly IRelyingPartyRepository _relyingPartyRepository;

        public SearchRelyingPartiesHandler(IRelyingPartyRepository relyingPartyRepository)
        {
            _relyingPartyRepository = relyingPartyRepository;
        }

        public async Task<JObject> Handle(SearchRelyingPartiesParameter parameter, CancellationToken cancellationToken)
        {
            var result = await _relyingPartyRepository.Search(parameter, cancellationToken);
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
