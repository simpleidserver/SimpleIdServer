// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.UI.Authenticate.Sms.Services
{
    public interface ISmsAuthService
    {
        Task<OAuthUser> Authenticate(string phoneNumber, long code, CancellationToken cancellationToken);
        Task<long> SendCode(string phoneNumber, CancellationToken cancellationToken);
    }
}
