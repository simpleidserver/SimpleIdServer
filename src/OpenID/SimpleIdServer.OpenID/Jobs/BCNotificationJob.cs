// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.Store;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Jobs
{
    public class BCNotificationJob
    {
        private readonly IBCAuthorizeRepository _bcAuthorizeRepository;
        private readonly IEnumerable<IBCNotificationHandler> _notificationHandlers;

        public BCNotificationJob(IBCAuthorizeRepository bcAuthorizeRepository, IEnumerable<IBCNotificationHandler> notificationHandlers)
        {
            _bcAuthorizeRepository = bcAuthorizeRepository;
            _notificationHandlers = notificationHandlers;
        }


        public async Task Execute(CancellationToken cancellationToken)
        {
            await NotifyConfirmedAuthorizeRequest(cancellationToken);
            await NotifyNotSentRejectedAuthorizeRequest(cancellationToken);
        }

        protected virtual async Task NotifyConfirmedAuthorizeRequest(CancellationToken cancellationToken)
        {
            var confirmedAuthorizeRequestLst = await _bcAuthorizeRepository.Query().Where(a => a.Status == Domains.BCAuthorizeStatus.Confirmed).ToListAsync(cancellationToken);
            foreach (var confirmedAuthorizeRequest in confirmedAuthorizeRequestLst)
            {
                var notificationHandler = _notificationHandlers.FirstOrDefault(n => n.NotificationMode == confirmedAuthorizeRequest.NotificationMode);
                if (notificationHandler == null)
                    continue;

                await notificationHandler.Notify(confirmedAuthorizeRequest, cancellationToken);
                if (notificationHandler.NotificationMode == SIDOpenIdConstants.StandardNotificationModes.Ping)
                    confirmedAuthorizeRequest.Pong();
                else
                    confirmedAuthorizeRequest.Send();
            }

            await _bcAuthorizeRepository.SaveChanges(cancellationToken);
        }

        protected virtual async Task NotifyNotSentRejectedAuthorizeRequest(CancellationToken cancellationToken)
        {
            var notSentRejectedAuthorizeRequestLst = await _bcAuthorizeRepository.Query().Where(bc => bc.Status == BCAuthorizeStatus.Rejected && bc.RejectionSentDateTime == null).ToListAsync(cancellationToken);
            foreach (var notSentRejectedAuthorizeRequest in notSentRejectedAuthorizeRequestLst)
            {
                var notificationHandler = _notificationHandlers.FirstOrDefault(n => n.NotificationMode == notSentRejectedAuthorizeRequest.NotificationMode);
                if (notificationHandler == null)
                    continue;

                await notificationHandler.Notify(notSentRejectedAuthorizeRequest, cancellationToken);
                notSentRejectedAuthorizeRequest.NotifyRejection();
            }

            await _bcAuthorizeRepository.SaveChanges(cancellationToken);
        }
    }
}
