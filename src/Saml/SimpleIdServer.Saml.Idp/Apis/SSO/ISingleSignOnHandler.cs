// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Idp.Apis.SSO
{
    public interface ISingleSignOnHandler
    {
        Task<SingleSignOnResult> Handle(SAMLRequestDto parameter, string userId, CancellationToken cancellationToken);
    }
}
