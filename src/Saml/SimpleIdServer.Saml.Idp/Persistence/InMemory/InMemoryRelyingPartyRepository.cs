// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Idp.Domains;
using SimpleIdServer.Saml.Idp.Persistence.Parameters;
using SimpleIdServer.Saml.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Idp.Persistence.InMemory
{
    public class InMemoryRelyingPartyRepository : IRelyingPartyRepository
    {
        private static Dictionary<string, string> MAPPING_RELYINGPARTY_TO_PROPERTYNAME = new Dictionary<string, string>
        {
            { "id", "Id" },
            { "create_datetime", "CreateDateTime" },
            { "update_datetime", "UpdateDateTime" }
        };
        private readonly ICollection<RelyingPartyAggregate> _relyingParties;

        public InMemoryRelyingPartyRepository(ICollection<RelyingPartyAggregate> relyingParties)
        {
            _relyingParties = relyingParties;
        }

        public Task<bool> Add(RelyingPartyAggregate relyingPartyAggregate, CancellationToken cancellationToken)
        {
            _relyingParties.Add((RelyingPartyAggregate)relyingPartyAggregate.Clone());
            return Task.FromResult(true);
        }

        public Task<RelyingPartyAggregate> Get(string id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_relyingParties.FirstOrDefault(r => r.Id == id));
        }

        public Task<SearchResult<RelyingPartyAggregate>> Search(SearchRelyingPartiesParameter parameter, CancellationToken cancellationToken)
        {
            var result = _relyingParties.AsQueryable();
            if (MAPPING_RELYINGPARTY_TO_PROPERTYNAME.ContainsKey(parameter.OrderBy))
            {
                result = result.InvokeOrderBy(MAPPING_RELYINGPARTY_TO_PROPERTYNAME[parameter.OrderBy], parameter.Order);
            }

            int totalLength = result.Count();
            result = result.Skip(parameter.StartIndex).Take(parameter.Count);
            var content = result.ToList();
            return Task.FromResult(new SearchResult<RelyingPartyAggregate>
            {
                StartIndex = parameter.StartIndex,
                Count = parameter.Count,
                TotalLength = totalLength,
                Content = content
            });
        }

        public Task<bool> Update(RelyingPartyAggregate relyingPartyAggregate, CancellationToken cancellationToken)
        {
            var record = _relyingParties.First(r => r.Id == relyingPartyAggregate.Id);
            _relyingParties.Remove(record);
            return Task.FromResult(true);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
