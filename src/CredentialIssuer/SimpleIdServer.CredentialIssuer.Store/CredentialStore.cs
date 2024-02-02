// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.CredentialIssuer.Domains;

namespace SimpleIdServer.CredentialIssuer.Store;

public interface ICredentialStore
{
    Task<Credential> GetByCredentialId(string credentialId, CancellationToken cancellationToken);
}

public class CredentialStore : ICredentialStore
{
    private readonly CredentialIssuerDbContext _dbContext;

    public CredentialStore(CredentialIssuerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Credential> GetByCredentialId(string credentialId, CancellationToken cancellationToken)
    {
        return _dbContext.Credentials
            .Include(c => c.Configuration)
            .SingleOrDefaultAsync(c => c.CredentialId == credentialId, cancellationToken);
    }
}
