// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.UI.Authenticate.Email.Services
{
    public interface IEmailAuthService
    {
        Task<OAuthUser> Authenticate(string email, long otpCode, CancellationToken cancellationToken);
        Task<long> SendCode(string email, CancellationToken cancellationToken);
    }
}
