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
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.ONLY_PINGORPOLL_MODE_CAN_BE_USED);
            }

            var authRequestId = context.Request.RequestData.GetAuthRequestId();
            if (string.IsNullOrWhiteSpace(authRequestId))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, AuthorizationRequestParameters.AuthReqId));

            var authRequest = await _bcAuthorizeRepository.Query().Include(a => a.Histories).FirstOrDefaultAsync(bc => bc.Id == authRequestId, cancellationToken);
            if (authRequest == null)
                throw new OAuthException(ErrorCodes.INVALID_GRANT, Global.InvalidAuthRequestId);

            if (authRequest.ClientId != openidClient.ClientId)
                throw new OAuthException(ErrorCodes.INVALID_GRANT, ErrorMessages.AUTH_REQUEST_CLIENT_NOT_AUTHORIZED);

            var currentDateTime = DateTime.UtcNow;
            var isSlowDown = currentDateTime <= authRequest.NextFetchTime;
            if (authRequest.LastStatus == BCAuthorizeStatus.Pending || isSlowDown)
            {
                if (isSlowDown)
                    throw new OAuthException(ErrorCodes.SLOW_DOWN, ErrorMessages.TOO_MANY_AUTH_REQUEST);

                authRequest.IncrementNextFetchTime();
                throw new OAuthException(ErrorCodes.AUTHORIZATION_PENDING, ErrorMessages.AUTH_REQUEST_NOT_CONFIRMED);
            }

            if (authRequest.LastStatus == BCAuthorizeStatus.Rejected)
                throw new OAuthException(ErrorCodes.ACCESS_DENIED, ErrorMessages.AUTH_REQUEST_REJECTED);

            if (authRequest.LastStatus == BCAuthorizeStatus.Sent)
                throw new OAuthException(ErrorCodes.INVALID_GRANT, ErrorMessages.AUTH_REQUEST_SENT);

            if (currentDateTime > authRequest.ExpirationDateTime)
                throw new OAuthException(ErrorCodes.EXPIRED_TOKEN, ErrorMessages.AUTH_REQUEST_EXPIRED);

            return authRequest;
        }
    }
}
