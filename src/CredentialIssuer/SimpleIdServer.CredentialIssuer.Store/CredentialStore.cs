// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.CredentialIssuer.Domains;

namespace SimpleIdServer.CredentialIssuer.Store;

public interface ICredentialStore
{
    Task<Credential> Get(string id, CancellationToken cancellationToken);
    Task<Credential> GetByCredentialId(string credentialId, CancellationToken cancellationToken);
    Task<List<Credential>> GetByCredentialConfigurationId(string credentialConfigurationId, CancellationToken cancellationToken);
    void Add(Credential credential);
    void Remove(Credential credential);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}

public class CredentialStore : ICredentialStore
{
    private readonly CredentialIssuerDbContext _dbContext;

    public CredentialStore(CredentialIssuerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Credential> Get(string id, CancellationToken cancellationToken)
    {
        return _dbContext.Credentials
            .Include(c => c.Configuration)
            .Include(c => c.Claims)
            .SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public Task<Credential> GetByCredentialId(string credentialId, CancellationToken cancellationToken)
    {
        return _dbContext.Credentials
            .Include(c => c.Configuration)
            .Include(c => c.Claims)
            .SingleOrDefaultAsync(c => c.CredentialId == credentialId, cancellationToken);
    }

    public Task<List<Credential>> GetByCredentialConfigurationId(string credentialConfigurationId, CancellationToken cancellationToken)
    {
        return _dbContext.Credentials
            .Include(c => c.Configuration)
            .Include(c => c.Claims)
            .Where(c => c.CredentialConfigurationId == credentialConfigurationId)
            .ToListAsync(cancellationToken);
    }

    public void Add(Credential credential)
        => _dbContext.Credentials.Add(credential);

    public void Remove(Credential credential)
        => _dbContext.Credentials.Remove(credential);

    public Task<int> SaveChanges(CancellationToken cancellationToken)
        => _dbContext.SaveChangesAsync(cancellationToken);
}