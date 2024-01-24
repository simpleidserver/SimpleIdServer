// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.CredentialIssuer.Domains;

namespace SimpleIdServer.CredentialIssuer.Store;

public interface ICredentialTemplateStore
{
    Task<List<CredentialTemplate>> GetAll(CancellationToken cancellationToken);
    Task<CredentialTemplate> Get(string id, CancellationToken cancellationToken);
}

public class CredentialTemplateStore : ICredentialTemplateStore
{
    public Task<List<CredentialTemplate>> GetAll(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<CredentialTemplate> Get(string id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
