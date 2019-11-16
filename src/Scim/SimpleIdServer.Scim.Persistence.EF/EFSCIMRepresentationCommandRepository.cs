// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Scim.Domain;
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

        public bool Add(SCIMRepresentation data)
        {
            _scimDbContext.SCIMRepresentationLst.Add(data);
            return true;
        }

        public bool Delete(SCIMRepresentation data)
        {
            var record = _scimDbContext.SCIMRepresentationLst.Find(data.Id);
            _scimDbContext.SCIMRepresentationLst.Remove(record);
            return true;
        }

        public bool Update(SCIMRepresentation data)
        {
            _scimDbContext.Entry(data).State = EntityState.Modified;
            return true;
        }

        public Task<int> SaveChanges()
        {
            return _scimDbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
        }
    }
}
