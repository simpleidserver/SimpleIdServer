// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Authenticate.Validations;

public interface IAlgClientSecretValidator
{
    bool IsValid(ClientSecret secret, string clientSecret);
    HashAlgs Alg { get; }
}
