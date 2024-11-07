// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar
{
    public class AuthenticationContextClassReferenceRepository : IAuthenticationContextClassReferenceRepository
    {
        private readonly DbContext _dbContext;

        public AuthenticationContextClassReferenceRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(AuthenticationContextClassReference record)
        {
            var newRecord = SugarAuthenticationContextClassReference.Transform(record);
            _dbContext.Client.InsertNav(newRecord)
                .Include(r => r.Realms)
                .ExecuteCommand();
        }

        public void Delete(AuthenticationContextClassReference record)
        {
            _dbContext.Client.Deleteable(new SugarAuthenticationContextClassReference
            {
                Id = record.Id
            }).ExecuteCommand();
        }

        public async Task<AuthenticationContextClassReference> Get(string realm, string id, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarAuthenticationContextClassReference>()
                .Includes(a => a.Realms)
                .Includes(a => a.RegistrationWorkflow)
                .FirstAsync(a => a.Realms.Any(r => r.RealmsName == realm) && a.Id == id, cancellationToken);
            return result?.ToDomain();
        }

        public async Task<List<AuthenticationContextClassReference>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarAuthenticationContextClassReference>()
                .Includes(a => a.Realms)
                .ToListAsync(cancellationToken);
            return result?.Select(r => r.ToDomain()).ToList();
        }

        public async Task<List<AuthenticationContextClassReference>> GetAll(string realm, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarAuthenticationContextClassReference>()
                .Includes(a => a.Realms)
                .Includes(a => a.RegistrationWorkflow)
                .Where(a => a.Realms.Any(r => r.RealmsName == realm))
                .OrderBy(a => a.Name)
                .ToListAsync(cancellationToken);
            return result.Select(r => r.ToDomain()).ToList();
        }

        public async Task<AuthenticationContextClassReference> GetByName(string realm, string name, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarAuthenticationContextClassReference>()
                .Includes(a => a.Realms)
                .Includes(a => a.RegistrationWorkflow)
                .FirstAsync(a => a.Realms.Any(r => r.RealmsName == realm) && a.Name == name, cancellationToken);
            return result?.ToDomain();
        }

        public async Task<List<AuthenticationContextClassReference>> GetByNames(List<string> names, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarAuthenticationContextClassReference>()
                .Includes(a => a.Realms)
                .Includes(a => a.RegistrationWorkflow)
                .Where(a => names.Contains(a.Name))
                .ToListAsync(cancellationToken);
            return result.Select(r => r.ToDomain()).ToList();
        }

        public async Task<List<AuthenticationContextClassReference>> GetByNames(string realm, List<string> names, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarAuthenticationContextClassReference>()
                .Includes(a => a.Realms)
                .Includes(a => a.RegistrationWorkflow)
                .Where(a => names.Contains(a.Name) && a.Realms.Any(r => r.RealmsName == realm))
                .ToListAsync(cancellationToken);
            return result.Select(r => r.ToDomain()).ToList();
        }

        public void Update(AuthenticationContextClassReference record)
        {
            _dbContext.Client.Updateable(SugarAuthenticationContextClassReference.Transform(record)).ExecuteCommand();
        }
    }
}
