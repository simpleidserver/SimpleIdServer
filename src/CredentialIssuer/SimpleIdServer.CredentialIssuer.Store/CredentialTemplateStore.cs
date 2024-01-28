// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.CredentialIssuer.Domains;

namespace SimpleIdServer.CredentialIssuer.Store;

public interface ICredentialTemplateStore
{
    Task<List<CredentialConfiguration>> GetAll(CancellationToken cancellationToken);
    Task<CredentialConfiguration> Get(string id, CancellationToken cancellationToken);
    Task<List<CredentialConfiguration>> Get(List<string> ids, CancellationToken cancellationToken);
}

public class CredentialTemplateStore : ICredentialTemplateStore
{
    public Task<List<CredentialConfiguration>> GetAll(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<CredentialConfiguration> Get(string id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<CredentialConfiguration> Get(List<string> ids, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
