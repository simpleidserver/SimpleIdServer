// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Helpers
{
    public interface IAmrHelper
    {
        Task<AuthenticationContextClassReference> FetchDefaultAcr(IEnumerable<string> requestedAcrValues, IEnumerable<AuthorizationRequestClaimParameter> requestedClaims, OpenIdClient openIdClient, CancellationToken cancellationToken);
        Task<AuthenticationContextClassReference> GetSupportedAcr(IEnumerable<string> requestedAcrValues, CancellationToken cancellationToken);
        string FetchNextAmr(AuthenticationContextClassReference acr, string currentAmr);
    }
}