// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.IdServer.Authenticate.Validations;

public interface IClientSecretValidator
{
    bool IsValid(Client client, string clientSecret);
}

public class ClientSecretValidator : IClientSecretValidator
{
    private readonly IEnumerable<IAlgClientSecretValidator> _validators;

    public ClientSecretValidator(IEnumerable<IAlgClientSecretValidator> validators)
    {
        _validators = validators;
    }

    public bool IsValid(Client client, string clientSecret)
    {
        if(string.IsNullOrWhiteSpace(clientSecret))
        {
            return false;
        }

        foreach(var cl in client.Secrets.Where(s => !s.IsInactive))
        {
            var validator = _validators.Single(v => v.Alg == cl.Alg);
            if(validator.IsValid(cl, clientSecret))
            {
                return true;
            }
        }

        return false;
    }
}
