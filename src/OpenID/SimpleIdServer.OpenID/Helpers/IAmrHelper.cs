// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OpenID.Domains;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Helpers
{
    public interface IAmrHelper
    {
        Task<AuthenticationContextClassReference> FetchDefaultAcr(IEnumerable<string> requestedAcrValues, OpenIdClient openidClient);
        string FetchNextAmr(AuthenticationContextClassReference acr, string currentAmr);
    }
}