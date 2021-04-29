// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.OpenID.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.BCDeviceRegistration
{
    public interface IBCDeviceRegistrationValidator
    {
        Task<OAuthUser> Validate(HandlerContext context, CancellationToken cancellationToken);
    }

    public class BCDeviceRegistrationValidator : IBCDeviceRegistrationValidator
    {
        private readonly IJwtParser _jwtParser;
        private readonly IOAuthUserQueryRepository _oauthUserQueryRepository;

        public BCDeviceRegistrationValidator(IJwtParser jwtParser, IOAuthUserQueryRepository oauthUserQueryRepository)
        {
            _jwtParser = jwtParser;
            _oauthUserQueryRepository = oauthUserQueryRepository;
        }

        public async Task<OAuthUser> Validate(HandlerContext context, CancellationToken cancellationToken)
        {
            var user = await ValidateIdTokenHint(context, cancellationToken);
            ValidateDeviceRegistrationToken(context);
            return user;
        }

        protected virtual async Task<OAuthUser> ValidateIdTokenHint(HandlerContext context, CancellationToken cancellationToken)
        {
            var idTokenHint = context.Request.RequestData.GetIdTokenHintFromAuthorizationRequest();
            if (string.IsNullOrWhiteSpace(idTokenHint))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(OAuth.ErrorMessages.MISSING_PARAMETER, AuthorizationRequestParameters.IdTokenHint));
            }

            var payload = await ExtractHint(idTokenHint);
            if (!payload.GetAudiences().Contains(context.Request.IssuerName))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_AUDIENCE_IDTOKENHINT);
            }
            return await CheckHint(payload);
        }

        protected virtual void ValidateDeviceRegistrationToken(HandlerContext context)
        {
            var deviceRegistrationToken = context.Request.RequestData.GetDeviceRegistrationToken();
            if (string.IsNullOrWhiteSpace(deviceRegistrationToken))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(OAuth.ErrorMessages.MISSING_PARAMETER, BCDeviceRegistrationRequestParameters.DeviceRegistrationToken));
            }
        }

        protected virtual async Task<JwsPayload> ExtractHint(string tokenHint)
        {
            if (!_jwtParser.IsJwsToken(tokenHint) && !_jwtParser.IsJweToken(tokenHint))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_IDTOKENHINT);
            }

            if (_jwtParser.IsJweToken(tokenHint))
            {
                tokenHint = await _jwtParser.Decrypt(tokenHint);
                if (string.IsNullOrWhiteSpace(tokenHint))
                {
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_IDTOKENHINT);
                }
            }

            return await _jwtParser.Unsign(tokenHint);
        }

        protected virtual async Task<OAuthUser> CheckHint(JwsPayload jwsPayload)
        {
            var exp = jwsPayload.GetExpirationTime();
            var currentDateTime = DateTime.UtcNow.ConvertToUnixTimestamp();
            if (currentDateTime > exp)
            {
                throw new OAuthException(ErrorCodes.EXPIRED_LOGIN_HINT_TOKEN, ErrorMessages.LOGIN_HINT_TOKEN_IS_EXPIRED);
            }

            var subject = jwsPayload.GetSub();
            var user = await _oauthUserQueryRepository.FindOAuthUserByClaim(Jwt.Constants.UserClaims.Subject, subject);
            if (user == null)
            {
                throw new OAuthException(ErrorCodes.UNKNOWN_USER_ID, string.Format(ErrorMessages.UNKNOWN_USER, subject));
            }

            return user;
        }
    }
}
