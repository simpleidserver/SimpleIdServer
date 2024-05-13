// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.EF;

public class AuthenticationContextClassReferenceRepository : IAuthenticationContextClassReferenceRepository
{
    private readonly StoreDbContext _dbContext;

    public AuthenticationContextClassReferenceRepository(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<AuthenticationContextClassReference> Get(string realm, string id, CancellationToken cancellationToken)
    {
        return _dbContext.Acrs
            .Include(a => a.Realms)
            .Include(a => a.RegistrationWorkflow)
            .SingleOrDefaultAsync(a => a.Realms.Any(r => r.Name == realm) && a.Id == id, cancellationToken);
    }

    public Task<AuthenticationContextClassReference> GetByName(string realm, string name, CancellationToken cancellationToken)
    {
        return _dbContext.Acrs
            .Include(a => a.Realms)
            .Include(a => a.RegistrationWorkflow)
            .SingleOrDefaultAsync(a => a.Realms.Any(r => r.Name == realm) && a.Name == name, cancellationToken);
    }

    public Task<List<AuthenticationContextClassReference>> GetAll(CancellationToken cancellationToken)
    {
        return _dbContext.Acrs
            .Include(a => a.Realms)
            .ToListAsync(cancellationToken);
    }

    public Task<List<AuthenticationContextClassReference>> GetAll(string realm, CancellationToken cancellationToken)
    {
        return _dbContext.Acrs
            .Include(a => a.Realms)
            .Include(a => a.RegistrationWorkflow)
            .Where(a => a.Realms.Any(r => r.Name == realm))
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);
    }

    public void Add(AuthenticationContextClassReference record) => _dbContext.Acrs.Add(record);

    public void Delete(AuthenticationContextClassReference record) => _dbContext.Acrs.Remove(record);

    public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
}