// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Token.Helpers;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Extensions;
using SimpleIdServer.OpenID.Options;
using SimpleIdServer.OpenID.Persistence;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.BCAuthorize
{
    public interface IBCAuthorizeRequestValidator
    {
        Task<OAuthUser> ValidateCreate(HandlerContext context, CancellationToken cancellationToken);
        Task<BCAcceptRequestValidationResult> ValidateConfirm(HandlerContext context, CancellationToken cancellationToken);
    }

    public class BCAuthorizeRequestValidator: IBCAuthorizeRequestValidator
    {
        private readonly IJwtParser _jwtParser;
        private readonly IOAuthUserQueryRepository _oauthUserQueryRepository;
        private readonly IBCAuthorizeRepository _bcAuthorizeRepository;
        private readonly OpenIDHostOptions _options;

        public BCAuthorizeRequestValidator(IJwtParser jwtParser, IOAuthUserQueryRepository oauthUserQueryRepository, IBCAuthorizeRepository bcAuthorizeRepository, IOptions<OpenIDHostOptions> options)
        {
            _jwtParser = jwtParser;
            _oauthUserQueryRepository = oauthUserQueryRepository;
            _bcAuthorizeRepository = bcAuthorizeRepository;
            _options = options.Value;
        }

        public virtual async Task<OAuthUser> ValidateCreate(HandlerContext context, CancellationToken cancellationToken)
        {
            // TODO : Check the signature of the authentication request.
            var tokens = new bool[]
            {
                string.IsNullOrWhiteSpace(context.Request.Data.GetLoginHintToken()),
                string.IsNullOrWhiteSpace(context.Request.Data.GetIdTokenHintFromAuthorizationRequest()),
                string.IsNullOrWhiteSpace(context.Request.Data.GetLoginHintFromAuthorizationRequest())
            };
            if(tokens.All(_ => _) || (tokens.Where(_ => !_).Count() > 1))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.ONE_HINT_MUST_BE_PASSED);
            }

            CheckScopes(context);
            CheckClientNotificationToken(context);
            var user = await CheckLoginHintToken(context);
            if (user == null)
            {
                user = await CheckIdTokenHint(context);
                if (user == null)
                {
                    user = await CheckLoginHint(context);
                }
            }

            CheckBindingMessage(context);
            CheckRequestedExpiry(context);
            return user;
        }

        public virtual async Task<BCAcceptRequestValidationResult> ValidateConfirm(HandlerContext context, CancellationToken cancellationToken)
        {
            var tokens = new bool[]
            {
                string.IsNullOrWhiteSpace(context.Request.Data.GetLoginHintToken()),
                string.IsNullOrWhiteSpace(context.Request.Data.GetIdTokenHintFromAuthorizationRequest()),
                string.IsNullOrWhiteSpace(context.Request.Data.GetLoginHintFromAuthorizationRequest())
            };
            if (tokens.All(_ => _) || (tokens.Where(_ => !_).Count() > 1))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.ONE_HINT_MUST_BE_PASSED);
            }

            var user = await CheckLoginHintToken(context);
            if (user == null)
            {
                user = await CheckIdTokenHint(context);
                if (user == null)
                {
                    user = await CheckLoginHint(context);
                }
            }

            var authRequestId = context.Request.Data.GetAuthRequestId();
            if (string.IsNullOrWhiteSpace(authRequestId))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(OAuth.ErrorMessages.MISSING_PARAMETER, DTOs.AuthorizationRequestParameters.AuthReqId));
            }

            var authRequest = await _bcAuthorizeRepository.Get(authRequestId, cancellationToken);
            if (authRequest == null)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_AUTH_REQUEST_ID);
            }

            var currentDateTime = DateTime.UtcNow;
            if (currentDateTime > authRequest.ExpirationDateTime)
            {
                throw new OAuthException(ErrorCodes.EXPIRED_TOKEN, string.Format(ErrorMessages.AUTH_REQUEST_EXPIRED, authRequestId));
            }

            return new BCAcceptRequestValidationResult(user, authRequest);
        }

        private void CheckScopes(HandlerContext context)
        {
            var requestedScopes = context.Request.Data.GetScopesFromAuthorizationRequest();
            if (!requestedScopes.Any())
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(OAuth.ErrorMessages.MISSING_PARAMETER, AuthorizationRequestParameters.Scope));
            }

            ScopeHelper.Validate(requestedScopes, context.Client.AllowedScopes.Select(s => s.Name));
        }

        private void CheckClientNotificationToken(HandlerContext context)
        {
            var clientNotificationToken = context.Request.Data.GetClientNotificationToken();
            if (string.IsNullOrWhiteSpace(clientNotificationToken))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(OAuth.ErrorMessages.MISSING_PARAMETER, DTOs.BCAuthenticationRequestParameters.ClientNotificationToken));
            }

            if (clientNotificationToken.Length > 1024)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.CLIENT_NOTIFICATION_TOKEN_MUST_NOT_EXCEED_1024);
            }

            if (System.Text.ASCIIEncoding.ASCII.GetByteCount(clientNotificationToken) * 8 < 128)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.CLIENT_NOTIFICATION_TOKEN_MUST_CONTAIN_AT_LEAST_128_BYTES);
            }
        }

        private async Task<OAuthUser> CheckLoginHintToken(HandlerContext context)
        {
            var loginHintToken = context.Request.Data.GetLoginHintToken();
            if (!string.IsNullOrWhiteSpace(loginHintToken))
            {
                var payload = await ExtractHint(loginHintToken);
                return await CheckHint(payload);
            }

            return null;
        }

        private async Task<OAuthUser> CheckIdTokenHint(HandlerContext context)
        {
            var idTokenHint = context.Request.Data.GetIdTokenHintFromAuthorizationRequest();
            if (!string.IsNullOrWhiteSpace(idTokenHint))
            {
                var payload = await ExtractHint(idTokenHint);
                if (!payload.GetAudiences().Contains(context.Request.IssuerName))
                {
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_AUDIENCE_IDTOKENHINT);
                }

                return await CheckHint(payload);
            }

            return null;
        }

        private async Task<OAuthUser> CheckLoginHint(HandlerContext context)
        {
            var loginHint = context.Request.Data.GetLoginHintFromAuthorizationRequest();
            if (!string.IsNullOrEmpty(loginHint))
            {
                var user = await _oauthUserQueryRepository.FindOAuthUserByClaim(Jwt.Constants.UserClaims.Subject, loginHint);
                if (user == null)
                {
                    throw new OAuthException(ErrorCodes.UNKNOWN_USER_ID, string.Format(ErrorMessages.UNKNOWN_USER, loginHint));
                }

                return user;
            }

            return null;
        }

        private void CheckBindingMessage(HandlerContext context)
        {
            var bindingMessage = context.Request.Data.GetBindingMessage();
            if (!string.IsNullOrWhiteSpace(bindingMessage) && bindingMessage.Count() > _options.MaxBindingMessageSize)
            {
                throw new OAuthException(ErrorCodes.INVALID_BINDING_MESSAGE, string.Format(ErrorMessages.BINDING_MESSAGE_MUST_NOT_EXCEED, _options.MaxBindingMessageSize));
            }
        }

        private void CheckRequestedExpiry(HandlerContext context)
        {
            var requestedExpiry = context.Request.Data.GetRequestedExpiry();
            if (requestedExpiry.HasValue && requestedExpiry.Value < 0)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.REQUESTED_EXPIRY_MUST_BE_POSITIVE);
            }
        }

        private async Task<OAuthUser> CheckHint(JwsPayload jwsPayload)
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

        protected async Task<JwsPayload> ExtractHint(string tokenHint)
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
    }
}
