// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Idp.Domains;
using SimpleIdServer.Saml.Idp.Persistence.Parameters;
using SimpleIdServer.Saml.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Idp.Persistence
{
    public interface IRelyingPartyRepository
    {
        Task<RelyingPartyAggregate> Get(string id, CancellationToken cancellationToken);
        Task<bool> Add(RelyingPartyAggregate relyingPartyAggregate, CancellationToken cancellationToken);
        Task<bool> Update(RelyingPartyAggregate relyingPartyAggregate, CancellationToken cancellationToken);
        Task<SearchResult<RelyingPartyAggregate>> Search(SearchRelyingPartiesParameter parameter, CancellationToken cancellationToken);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }
}
