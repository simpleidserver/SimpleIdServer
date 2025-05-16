// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Authenticate.Validations;

public class Md5ClientSecretValidator : IAlgClientSecretValidator
{
    public HashAlgs Alg => HashAlgs.MD5;

    public bool IsValid(ClientSecret secret, string clientSecret)
    {
        return secret.Value == ClientSecret.Create(clientSecret, Alg).Value;
    }
}
