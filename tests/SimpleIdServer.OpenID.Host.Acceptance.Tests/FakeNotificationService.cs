// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OpenID.Api.BCAuthorize;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Host.Acceptance.Tests
{
    public class FakeNotificationService : IBCNotificationService
    {
        public Task Notify(HandlerContext handlerContext, string authReqId, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
