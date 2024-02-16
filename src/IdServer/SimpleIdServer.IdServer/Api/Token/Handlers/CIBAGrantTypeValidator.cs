// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Store;
using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.Handlers
{
    public interface ICIBAGrantTypeValidator
    {
        Task<Domains.BCAuthorize> Validate(HandlerContext context, CancellationToken cancellationToken);
    }

    public class CIBAGrantTypeValidator : ICIBAGrantTypeValidator
    {
        private readonly IBCAuthorizeRepository _bcAuthorizeRepository;

        public CIBAGrantTypeValidator(IBCAuthorizeRepository bcAuthorizeRepository)
        {
            _bcAuthorizeRepository = bcAuthorizeRepository;
        }

        public async Task<Domains.BCAuthorize> Validate(HandlerContext context, CancellationToken cancellationToken)
        {
            Client openidClient = context.Client;
            if (openidClient.BCTokenDeliveryMode != Constants.StandardNotificationModes.Ping
                && openidClient.BCTokenDeliveryMode != Constants.StandardNotificationModes.Poll)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.OnlyPingOrPollModeCanBeUsed);
            }

            var authRequestId = context.Request.RequestData.GetAuthRequestId();
            if (string.IsNullOrWhiteSpace(authRequestId))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, AuthorizationRequestParameters.AuthReqId));

            var authRequest = await _bcAuthorizeRepository.Query().Include(a => a.Histories).FirstOrDefaultAsync(bc => bc.Id == authRequestId, cancellationToken);
            if (authRequest == null)
                throw new OAuthException(ErrorCodes.INVALID_GRANT, Global.InvalidAuthRequestId);

            if (authRequest.ClientId != openidClient.ClientId)
                throw new OAuthException(ErrorCodes.INVALID_GRANT, Global.AuthRequestClientNotAuthorized);

            var currentDateTime = DateTime.UtcNow;
            var isSlowDown = currentDateTime <= authRequest.NextFetchTime;
            if (authRequest.LastStatus == BCAuthorizeStatus.Pending || isSlowDown)
            {
                if (isSlowDown)
                    throw new OAuthException(ErrorCodes.SLOW_DOWN, Global.TooManyAuthRequest);

                authRequest.IncrementNextFetchTime();
                throw new OAuthException(ErrorCodes.AUTHORIZATION_PENDING, Global.AuthRequestNotConfirmed);
            }

            if (authRequest.LastStatus == BCAuthorizeStatus.Rejected)
                throw new OAuthException(ErrorCodes.ACCESS_DENIED, Global.AuthRequestRejected);

            if (authRequest.LastStatus == BCAuthorizeStatus.Sent)
                throw new OAuthException(ErrorCodes.INVALID_GRANT, Global.AuthRequestSent);

            if (currentDateTime > authRequest.ExpirationDateTime)
                throw new OAuthException(ErrorCodes.EXPIRED_TOKEN, Global.AuthRequestExpired);

            return authRequest;
        }
    }
}
