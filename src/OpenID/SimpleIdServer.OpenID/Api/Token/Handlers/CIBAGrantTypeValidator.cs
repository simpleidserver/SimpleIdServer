// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.OpenID.Extensions;
using SimpleIdServer.OpenID.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.Token.Handlers
{
    public class CIBAGrantTypeValidator : ICIBAGrantTypeValidator
    {
        private readonly IBCAuthorizeRepository _bcAuthorizeRepository;

        public CIBAGrantTypeValidator(IBCAuthorizeRepository bcAuthorizeRepository)
        {
            _bcAuthorizeRepository = bcAuthorizeRepository;
        }

        public async Task<Domains.BCAuthorize> Validate(HandlerContext context, CancellationToken cancellationToken)
        {
            var openidClient = context.Client as OpenIdClient;
            if (openidClient.BCTokenDeliveryMode != SIDOpenIdConstants.StandardNotificationModes.Ping
                && openidClient.BCTokenDeliveryMode != SIDOpenIdConstants.StandardNotificationModes.Poll)
            {
                throw new OAuthException(OAuth.ErrorCodes.INVALID_REQUEST, ErrorMessages.ONLY_PINGORPUSH_MODE_CAN_BE_USED);
            }

            var authRequestId = context.Request.Data.GetAuthRequestId();
            if (string.IsNullOrWhiteSpace(authRequestId))
            {
                throw new OAuthException(OAuth.ErrorCodes.INVALID_REQUEST, string.Format(OAuth.ErrorMessages.MISSING_PARAMETER, AuthorizationRequestParameters.AuthReqId));
            }

            var authRequest = await _bcAuthorizeRepository.Get(authRequestId, cancellationToken);
            if (authRequest == null)
            {
                throw new OAuthException(OAuth.ErrorCodes.INVALID_GRANT, ErrorMessages.INVALID_AUTH_REQUEST_ID);
            }

            var currentDateTime = DateTime.UtcNow;
            var isSlowDown = currentDateTime <= authRequest.NextFetchTime;
            if (authRequest.Status == Domains.BCAuthorizeStatus.Pending || isSlowDown)
            {
                if (isSlowDown)
                {
                    throw new OAuthException(OAuth.ErrorCodes.SLOW_DOWN, string.Format(ErrorMessages.TOO_MANY_AUTH_REQUEST, authRequestId));
                }

                authRequest.IncrementNextFetchTime();
                await _bcAuthorizeRepository.Update(authRequest, cancellationToken);
                await _bcAuthorizeRepository.SaveChanges(cancellationToken);
                throw new OAuthException(OAuth.ErrorCodes.AUTHORIZATION_PENDING, string.Format(ErrorMessages.AUTH_REQUEST_NOT_CONFIRMED, authRequestId));
            }

            if (authRequest.Status == BCAuthorizeStatus.Rejected)
            {
                throw new OAuthException(OAuth.ErrorCodes.ACCESS_DENIED, string.Format(ErrorMessages.AUTH_REQUEST_REJECTED, authRequestId));
            }

            if (authRequest.Status == BCAuthorizeStatus.Sent)
            {
                throw new OAuthException(OAuth.ErrorCodes.INVALID_GRANT, string.Format(ErrorMessages.AUTH_REQUEST_SENT, authRequestId));
            }

            if (currentDateTime > authRequest.ExpirationDateTime)
            {
                throw new OAuthException(OAuth.ErrorCodes.EXPIRED_TOKEN, string.Format(ErrorMessages.AUTH_REQUEST_EXPIRED, authRequestId));
            }

            return authRequest;
        }
    }
}
