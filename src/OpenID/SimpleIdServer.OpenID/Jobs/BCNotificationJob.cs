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
            await NotifyConfirmedAuthorizeRequest(cancellationToken);
            await NotifyNotSentRejectedAuthorizeRequest(cancellationToken);
        }

        protected virtual async Task NotifyConfirmedAuthorizeRequest(CancellationToken cancellationToken)
        {
            var confirmedAuthorizeRequestLst = await _bcAuthorizeRepository.GetConfirmedAuthorizationRequest(cancellationToken);
            foreach (var confirmedAuthorizeRequest in confirmedAuthorizeRequestLst)
            {
                var notificationHandler = _notificationHandlers.FirstOrDefault(n => n.NotificationMode == confirmedAuthorizeRequest.NotificationMode);
                if (notificationHandler == null)
                {
                    continue;
                }

                await notificationHandler.Notify(confirmedAuthorizeRequest, cancellationToken);
                if (notificationHandler.NotificationMode == SIDOpenIdConstants.StandardNotificationModes.Ping)
                {
                    confirmedAuthorizeRequest.Pong();
                }
                else
                {
                    confirmedAuthorizeRequest.Send();
                }

                await _bcAuthorizeRepository.Update(confirmedAuthorizeRequest, cancellationToken);
            }
        }

        protected virtual async Task NotifyNotSentRejectedAuthorizeRequest(CancellationToken cancellationToken)
        {
            var notSentRejectedAuthorizeRequestLst = await _bcAuthorizeRepository.GetNotSentRejectedAuthorizationRequest(cancellationToken);
            foreach (var notSentRejectedAuthorizeRequest in notSentRejectedAuthorizeRequestLst)
            {
                var notificationHandler = _notificationHandlers.FirstOrDefault(n => n.NotificationMode == notSentRejectedAuthorizeRequest.NotificationMode);
                if (notificationHandler == null)
                {
                    continue;
                }

                await notificationHandler.Notify(notSentRejectedAuthorizeRequest, cancellationToken);
                notSentRejectedAuthorizeRequest.NotifyRejection();
                await _bcAuthorizeRepository.Update(notSentRejectedAuthorizeRequest, cancellationToken);
            }
        }
    }
}
