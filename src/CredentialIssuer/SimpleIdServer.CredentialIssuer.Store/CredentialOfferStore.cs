// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.CredentialIssuer.Domains;

namespace SimpleIdServer.CredentialIssuer.Store;

public interface ICredentialOfferStore
{
    void Add(CredentialOfferRecord credentialOffer);
    Task<CredentialOfferRecord> Get(string id, CancellationToken cancellationToken);
    Task<CredentialOfferRecord> GetByIssuerState(string issuerState, CancellationToken cancellationToken);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}

public class CredentialOfferStore : ICredentialOfferStore
{
    private readonly CredentialIssuerDbContext _dbContext;

    public CredentialOfferStore(CredentialIssuerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(CredentialOfferRecord credentialOffer)
    {
        _dbContext.CredentialOfferRecords.Add(credentialOffer);
    }

    public Task<CredentialOfferRecord> Get(string id, CancellationToken cancellationToken)
    {
        return _dbContext.CredentialOfferRecords.SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public Task<CredentialOfferRecord> GetByIssuerState(string issuerState, CancellationToken cancellationToken)
    {
        return _dbContext.CredentialOfferRecords.SingleOrDefaultAsync(c => c.IssuerState == issuerState, cancellationToken);
    }

    public Task<int> SaveChanges(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
