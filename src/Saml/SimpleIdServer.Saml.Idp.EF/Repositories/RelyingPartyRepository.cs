// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Saml.Idp.Domains;
using SimpleIdServer.Saml.Idp.Persistence;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Idp.EF.Repositories
{
    public class RelyingPartyRepository : IRelyingPartyRepository
    {
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
    }
}
