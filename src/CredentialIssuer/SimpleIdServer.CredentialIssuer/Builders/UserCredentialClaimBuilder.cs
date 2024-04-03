// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.CredentialIssuer.Domains;
using System;

namespace SimpleIdServer.CredentialIssuer.Builders;

public static class UserCredentialClaimBuilder
{
    public static UserCredentialClaim Build(string subject, string name, string value) => new UserCredentialClaim
    {
        Id = Guid.NewGuid().ToString(),
        Subject = subject,
        Name = name,
        Value = value
    };
}
