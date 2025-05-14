// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System;

namespace SimpleIdServer.IdServer.Authenticate.Validations;

public class PlainTextClientSecretValidator : IAlgClientSecretValidator
{
    public bool IsValid(Client client, string clientSecret)
    {
        return string.Compare(client.ClientSecret, clientSecret, StringComparison.CurrentCultureIgnoreCase) == 0;
    }
}
