// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.CredentialIssuer.Domains;

namespace SimpleIdServer.CredentialIssuer.Store;

public interface ICredentialConfigurationStore
{
    Task<List<CredentialConfiguration>> GetAll(CancellationToken cancellationToken);
    Task<CredentialConfiguration> GetByType(string type, CancellationToken cancellationToken);
    Task<List<CredentialConfiguration>> Get(List<string> ids, CancellationToken cancellationToken);
}

public class CredentialConfigurationStore : ICredentialConfigurationStore
{
    private readonly CredentialIssuerDbContext _dbContext;

    public CredentialConfigurationStore(CredentialIssuerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<CredentialConfiguration> GetByType(string type, CancellationToken cancellationToken)
    {
        return _dbContext.CredentialConfigurations
            .Include(c => c.Claims).ThenInclude(c => c.Translations)
            .Include(c => c.Displays)
            .SingleOrDefaultAsync(c => c.Type == type, cancellationToken);
    }

    public Task<List<CredentialConfiguration>> Get(List<string> ids, CancellationToken cancellationToken)
    {
        return _dbContext.CredentialConfigurations
            .Include(c => c.Claims).ThenInclude(c => c.Translations)
            .Include(c => c.Displays)
            .Where(c => ids.Contains(c.Id))
            .ToListAsync(cancellationToken);
    }

    public Task<List<CredentialConfiguration>> GetAll(CancellationToken cancellationToken)
    {
        return _dbContext.CredentialConfigurations
            .Include(c => c.Claims).ThenInclude(c => c.Translations)
            .Include(c => c.Displays)
            .ToListAsync(cancellationToken);
    }
}
