// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Builders;

public class CredentialIssuerClientBuilder
{
    private readonly Client _client;

    public CredentialIssuerClientBuilder(Client client)
    {
        _client = client;
    }

    public Client Build() => _client;
}
