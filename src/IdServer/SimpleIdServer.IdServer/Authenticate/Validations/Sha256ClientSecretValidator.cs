// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Authenticate.Validations;

public class Sha256ClientSecretValidator : IAlgClientSecretValidator
{
    public bool IsValid(Client client, string clientSecret)
    {
        return new ClientSecret(clientSecret).Sha256() == client.ClientSecret;
    }
}
