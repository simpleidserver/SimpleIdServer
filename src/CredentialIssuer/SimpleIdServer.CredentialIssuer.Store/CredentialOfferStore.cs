// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.CredentialIssuer.Domains;

namespace SimpleIdServer.CredentialIssuer.Store;

public interface ICredentialOfferStore
{
    void Add(CredentialOfferRecord credentialOffer);
    Task<CredentialOfferRecord> Get(string id, CancellationToken cancellationToken);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}

public class CredentialOfferStore : ICredentialOfferStore
{
    public void Add(CredentialOfferRecord credentialOffer)
    {
        throw new NotImplementedException();
    }

    public Task<CredentialOfferRecord> Get(string id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveChanges(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
