// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores
{
    public interface IClientRepository
    {
        Task<Client> GetByClientId(string realm, string clientId, CancellationToken cancellationToken);
        Task<List<Client>> GetByClientIdsAndExistingBackchannelLogoutUri(string realm, List<string> clientIds, CancellationToken cancellationToken);
        Task<List<Client>> GetAll(string realm, CancellationToken cancellationToken);
        Task<List<Client>> GetAll(string realm, List<string> clientIds, CancellationToken cancellationToken);
        Task<SearchResult<Client>> Search(string realm, SearchRequest request, CancellationToken cancellationToken);
        void Delete(Client client);
        void Add(Client client);
        void DeleteRange(IEnumerable<Client> clients);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }
}
