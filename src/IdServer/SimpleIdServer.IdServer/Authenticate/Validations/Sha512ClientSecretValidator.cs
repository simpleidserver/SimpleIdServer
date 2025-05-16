// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Authenticate.Validations;

public class Sha512ClientSecretValidator : IAlgClientSecretValidator
{
    public HashAlgs Alg => HashAlgs.SHA512;

    public bool IsValid(ClientSecret secret, string clientSecret)
    {
        return secret.Value == ClientSecret.Create(clientSecret, Alg).Value;
    }
}
