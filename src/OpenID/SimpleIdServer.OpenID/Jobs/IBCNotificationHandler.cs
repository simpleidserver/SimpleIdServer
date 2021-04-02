// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.OpenID.Domains;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Jobs
{
    public interface IBCNotificationHandler
    {
        string NotificationMode { get; }
        Task<bool> Notify(BCAuthorize bcAuthorize, CancellationToken cancellationToken);
    }
}
