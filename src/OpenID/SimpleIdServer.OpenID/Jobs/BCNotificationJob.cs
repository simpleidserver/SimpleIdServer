// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.OpenID.Infrastructures.Jobs;
using SimpleIdServer.OpenID.Infrastructures.Locks;
using SimpleIdServer.OpenID.Options;
using SimpleIdServer.OpenID.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Jobs
{
    public class BCNotificationJob : BaseScheduledJob
    {
        private readonly IBCAuthorizeRepository _bcAuthorizeRepository;
        private readonly IEnumerable<IBCNotificationHandler> _notificationHandlers;

        public BCNotificationJob(
            IBCAuthorizeRepository bcAuthorizeRepository,
            IEnumerable<IBCNotificationHandler> notificationHandlers,
            IOptions<OpenIDHostOptions> options, 
            ILogger<BaseScheduledJob> logger, 
            IDistributedLock distributedLock) : base(options, logger, distributedLock)
        {
            _bcAuthorizeRepository = bcAuthorizeRepository;
            _notificationHandlers = notificationHandlers;
        }

        protected override string LockName => "authorize-request-notification";

        protected override async Task Execute(CancellationToken cancellationToken)
        {
            var pendingAuthorizeRequestLst = await _bcAuthorizeRepository.GetConfirmedAuthorizeRequest(cancellationToken);
            foreach(var pendingAuthorizeRequest in pendingAuthorizeRequestLst)
            {
                var notificationHandler = _notificationHandlers.FirstOrDefault(n => n.NotificationMode == pendingAuthorizeRequest.NotificationMode);
                if (notificationHandler == null)
                {
                    continue;
                }

                if (await notificationHandler.Notify(pendingAuthorizeRequest, cancellationToken))
                {
                    pendingAuthorizeRequest.Notify();
                    await _bcAuthorizeRepository.Update(pendingAuthorizeRequest, cancellationToken);
                }
            }

            await _bcAuthorizeRepository.SaveChanges(cancellationToken);
        }
    }
}
