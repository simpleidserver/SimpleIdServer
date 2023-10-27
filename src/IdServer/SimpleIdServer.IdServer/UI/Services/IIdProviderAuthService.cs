// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Security.Claims;

namespace SimpleIdServer.IdServer.UI.Services
{
    public interface IIdProviderAuthService
    {
        string Name { get; }
        bool Authenticate(User user, ICollection<Claim> claims, IdentityProvisioning identityProvisioning, string password);
    }
}
