// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.CredentialIssuer.Domains;

namespace SimpleIdServer.CredentialIssuer.Store;

public interface ICredentialConfigurationStore
{
    Task<List<CredentialConfiguration>> GetAll(CancellationToken cancellationToken);
    Task<CredentialConfiguration> GetByTypeAndFormat(string type, string format, CancellationToken cancellationToken);
    Task<CredentialConfiguration> GetByServerId(string id, CancellationToken cancellationToken);
    Task<List<CredentialConfiguration>> GetByServerIds(List<string> ids, CancellationToken cancellationToken);
    Task<CredentialConfiguration> Get(string id, CancellationToken cancellationToken);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}

public class CredentialConfigurationStore : ICredentialConfigurationStore
{
    private readonly CredentialIssuerDbContext _dbContext;

    public CredentialConfigurationStore(CredentialIssuerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<CredentialConfiguration> Get(string id, CancellationToken cancellationToken)
    {
        return _dbContext.CredentialConfigurations
            .Include(c => c.Claims).ThenInclude(c => c.Translations)
            .Include(c => c.Displays)
            .SingleOrDefaultAsync(c => c.Id == id);
    }

    public Task<CredentialConfiguration> GetByTypeAndFormat(string type, string format, CancellationToken cancellationToken)
    {
        return _dbContext.CredentialConfigurations
            .Include(c => c.Claims).ThenInclude(c => c.Translations)
            .Include(c => c.Displays)
            .SingleOrDefaultAsync(c => c.Type == type && c.Format == format, cancellationToken);
    }

    public Task<CredentialConfiguration> GetByServerId(string id, CancellationToken cancellationToken)
    {
        return _dbContext.CredentialConfigurations
            .Include(c => c.Claims).ThenInclude(c => c.Translations)
            .Include(c => c.Displays)
            .SingleOrDefaultAsync(c => id == c.ServerId, cancellationToken);
    }

    public Task<List<CredentialConfiguration>> GetByServerIds(List<string> ids, CancellationToken cancellationToken)
    {
        return _dbContext.CredentialConfigurations
            .Include(c => c.Claims).ThenInclude(c => c.Translations)
            .Include(c => c.Displays)
            .Where(c => ids.Contains(c.ServerId))
            .ToListAsync(cancellationToken);
    }

    public Task<List<CredentialConfiguration>> GetAll(CancellationToken cancellationToken)
    {
        return _dbContext.CredentialConfigurations
            .Include(c => c.Claims).ThenInclude(c => c.Translations)
            .Include(c => c.Displays)
            .ToListAsync(cancellationToken);
    }

    public Task<int> SaveChanges(CancellationToken cancellationToken)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
