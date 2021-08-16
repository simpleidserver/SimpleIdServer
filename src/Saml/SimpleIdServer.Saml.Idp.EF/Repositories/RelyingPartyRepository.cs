// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Idp.Domains;
using SimpleIdServer.Saml.Idp.Persistence;
using SimpleIdServer.Saml.Idp.Persistence.Parameters;
using SimpleIdServer.Saml.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Idp.EF.Repositories
{
    public class RelyingPartyRepository : IRelyingPartyRepository
    {
        private static Dictionary<string, string> MAPPING_RELYINGPARTY_TO_PROPERTYNAME = new Dictionary<string, string>
        {
            { "id", "Id" },
            { "create_datetime", "CreateDateTime" },
            { "update_datetime", "UpdateDateTime" }
        };
        private readonly SamlIdpDBContext _dbContext;

        public RelyingPartyRepository(SamlIdpDBContext samlIdpDBContext)
        {
            _dbContext = samlIdpDBContext;
        }

        public Task<RelyingPartyAggregate> Get(string id, CancellationToken cancellationToken)
        {
            return GetRelyingParties().FirstOrDefaultAsync(rp => rp.Id == id, cancellationToken);
        }

        public Task<bool> Add(RelyingPartyAggregate relyingPartyAggregate, CancellationToken cancellationToken)
        {
            _dbContext.RelyingParties.Add(relyingPartyAggregate);
            return Task.FromResult(true);
        }

        public Task<bool> Update(RelyingPartyAggregate relyingPartyAggregate, CancellationToken cancellationToken)
        {
            _dbContext.RelyingParties.Update(relyingPartyAggregate);
            return Task.FromResult(true);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public IQueryable<RelyingPartyAggregate> GetRelyingParties()
        {
            return _dbContext.RelyingParties.Include(r => r.ClaimMappings);
        }

        public async Task<SearchResult<RelyingPartyAggregate>> Search(SearchRelyingPartiesParameter parameter, CancellationToken cancellationToken)
        {
            var result = _dbContext.RelyingParties.AsQueryable();
            if (MAPPING_RELYINGPARTY_TO_PROPERTYNAME.ContainsKey(parameter.OrderBy))
            {
                result = result.InvokeOrderBy(MAPPING_RELYINGPARTY_TO_PROPERTYNAME[parameter.OrderBy], parameter.Order);
            }

            int totalLength = result.Count();
            result = result.Skip(parameter.StartIndex).Take(parameter.Count);
            ICollection<RelyingPartyAggregate> content = await result.ToListAsync(cancellationToken);
            return new SearchResult<RelyingPartyAggregate>
            {
                StartIndex = parameter.StartIndex,
                Count = parameter.Count,
                TotalLength = totalLength,
                Content = content
            };
        }
    }
}
