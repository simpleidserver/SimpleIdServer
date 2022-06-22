// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Scim.Domains;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.EF
{
    public class EFSCIMRepresentationCommandRepository : ISCIMRepresentationCommandRepository
    {
        private readonly SCIMDbContext _scimDbContext;

        public EFSCIMRepresentationCommandRepository(SCIMDbContext scimDbContext)
        {
            _scimDbContext = scimDbContext;
        }

        public async Task<SCIMRepresentation> Get(string id, CancellationToken token = default)
        {
            var result = await _scimDbContext.SCIMRepresentationLst
                .Include(r => r.FlatAttributes)
                .Include(r => r.Schemas).ThenInclude(s => s.Attributes).FirstOrDefaultAsync(r => r.Id == id, token);
            return result;
        }

        public async Task<ITransaction> StartTransaction(CancellationToken token)
        {
            var transaction = await _scimDbContext.Database.BeginTransactionAsync(token);
            return new EFTransaction(_scimDbContext, transaction);
        }

        public Task<bool> Add(SCIMRepresentation data, CancellationToken token)
        {
            _scimDbContext.SCIMRepresentationLst.Add(data);
            return Task.FromResult(true);
        }

        public Task<bool> Delete(SCIMRepresentation data, CancellationToken token)
        {
            _scimDbContext.SCIMRepresentationLst.Remove(data);
            return Task.FromResult(true);
        }

        public Task<bool> Update(SCIMRepresentation data, CancellationToken token)
        {
            _scimDbContext.SCIMRepresentationLst.Update(data);
            return Task.FromResult(true);
        }
    }
}
