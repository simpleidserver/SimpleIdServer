// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Idp.Domains;
using SimpleIdServer.Saml.Idp.Persistence;
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

        public Task<bool> Add(RelyingPartyAggregate relyingPartyAggregate, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<RelyingPartyAggregate> Get(string id, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Update(RelyingPartyAggregate relyingPartyAggregate, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
