// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains.Clients;
using SimpleIdServer.OpenID.Domains.ACRs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Helpers
{
    public interface IAmrHelper
    {
        Task<AuthenticationContextClassReference> FetchDefaultAcr(IEnumerable<string> requestedAcrValues, OAuthClient oauthClient);
        string FetchNextAmr(AuthenticationContextClassReference acr, string currentAmr);
    }
}