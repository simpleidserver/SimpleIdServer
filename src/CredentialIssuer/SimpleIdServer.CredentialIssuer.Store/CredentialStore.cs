// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.CredentialIssuer.Domains;

namespace SimpleIdServer.CredentialIssuer.Store;

public interface ICredentialStore
{
    Task<Credential> GetByCredentialId(string credentialId, CancellationToken cancellationToken);
}

public class CredentialStore : ICredentialStore
{
    public Task<Credential> GetByCredentialId(string credentialId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
