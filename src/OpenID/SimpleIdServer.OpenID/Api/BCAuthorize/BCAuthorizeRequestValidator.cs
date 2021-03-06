﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Jwt.Jws;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Token.Helpers;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.DTOs;
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
        Task<Domains.BCAuthorize> ValidateReject(HandlerContext context, CancellationToken cancellationToken);
    }

    public class BCAuthorizeRequestValidator: IBCAuthorizeRequestValidator
    {
        private readonly IJwtParser _jwtParser;
        private readonly IOAuthUserRepository _oauthUserRepository;
        private readonly IBCAuthorizeRepository _bcAuthorizeRepository;
        private readonly IRequestObjectValidator _requestObjectValidator;
        private readonly OpenIDHostOptions _options;

        public BCAuthorizeRequestValidator(
            IJwtParser jwtParser,
            IOAuthUserRepository oauthUserRepository, 
            IBCAuthorizeRepository bcAuthorizeRepository,
            IRequestObjectValidator requestObjectValidator,
            IOptions<OpenIDHostOptions> options)
        {
            _jwtParser = jwtParser;
            _oauthUserRepository = oauthUserRepository;
            _bcAuthorizeRepository = bcAuthorizeRepository;
            _requestObjectValidator = requestObjectValidator;
            _options = options.Value;
        }

        public virtual async Task<OAuthUser> ValidateCreate(HandlerContext context, CancellationToken cancellationToken)
        {
            await CheckRequestObject(context, cancellationToken);
            var tokens = new bool[]
            {
                string.IsNullOrWhiteSpace(context.Request.RequestData.GetLoginHintToken()),
                string.IsNullOrWhiteSpace(context.Request.RequestData.GetIdTokenHintFromAuthorizationRequest()),
                string.IsNullOrWhiteSpace(context.Request.RequestData.GetLoginHintFromAuthorizationRequest())
            };
            if(tokens.All(_ => _) || (tokens.Where(_ => !_).Count() > 1))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.ONE_HINT_MUST_BE_PASSED);
            }

            CheckScopes(context);
            CheckClientNotificationToken(context);
            var user = await CheckLoginHintToken(context, cancellationToken);
            if (user == null)
            {
                user = await CheckIdTokenHint(context, cancellationToken);
                if (user == null)
                {
                    user = await CheckLoginHint(context, cancellationToken);
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
                string.IsNullOrWhiteSpace(context.Request.RequestData.GetLoginHintToken()),
                string.IsNullOrWhiteSpace(context.Request.RequestData.GetIdTokenHintFromAuthorizationRequest()),
                string.IsNullOrWhiteSpace(context.Request.RequestData.GetLoginHintFromAuthorizationRequest())
            };
            if (tokens.All(_ => _) || (tokens.Where(_ => !_).Count() > 1))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.ONE_HINT_MUST_BE_PASSED);
            }

            var user = await CheckLoginHintToken(context, cancellationToken);
            if (user == null)
            {
                user = await CheckIdTokenHint(context, cancellationToken);
                if (user == null)
                {
                    user = await CheckLoginHint(context, cancellationToken);
                }
            }

            var authRequestId = context.Request.RequestData.GetAuthRequestId();
            if (string.IsNullOrWhiteSpace(authRequestId))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(OAuth.ErrorMessages.MISSING_PARAMETER, DTOs.AuthorizationRequestParameters.AuthReqId));
            }

            var authRequest = await _bcAuthorizeRepository.Get(authRequestId, cancellationToken);
            if (authRequest == null)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_AUTH_REQUEST_ID);
            }

            var permissionIds = context.Request.RequestData.GetPermissionIds();
            var unknownPermissionIds = permissionIds.Where(pid => !authRequest.Permissions.Any(p => p.PermissionId == pid));
            if (unknownPermissionIds.Any())
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_PERMISSIONS, string.Join(",", unknownPermissionIds)));
            }

            return new BCAcceptRequestValidationResult(user, authRequest);
        }

        public virtual async Task<Domains.BCAuthorize> ValidateReject(HandlerContext context, CancellationToken cancellationToken)
        {
            var tokens = new bool[]
            {
                string.IsNullOrWhiteSpace(context.Request.RequestData.GetLoginHintToken()),
                string.IsNullOrWhiteSpace(context.Request.RequestData.GetIdTokenHintFromAuthorizationRequest()),
                string.IsNullOrWhiteSpace(context.Request.RequestData.GetLoginHintFromAuthorizationRequest())
            };
            if (tokens.All(_ => _) || (tokens.Where(_ => !_).Count() > 1))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.ONE_HINT_MUST_BE_PASSED);
            }

            var user = await CheckLoginHintToken(context, cancellationToken);
            if (user == null)
            {
                user = await CheckIdTokenHint(context, cancellationToken);
                if (user == null)
                {
                    user = await CheckLoginHint(context, cancellationToken);
                }
            }

            var authReqId = context.Request.RequestData.GetAuthRequestId();
            if (string.IsNullOrWhiteSpace(authReqId))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(OAuth.ErrorMessages.MISSING_PARAMETER, DTOs.AuthorizationRequestParameters.AuthReqId));
            }

            var authRequest = await _bcAuthorizeRepository.Get(authReqId, cancellationToken);
            if (authRequest == null)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_AUTH_REQUEST_ID);
            }

            if (authRequest.UserId != user.Id)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.AUTH_REQUEST_NOT_AUTHORIZED_TO_REJECT);
            }

            return authRequest;
        }

        private async Task CheckRequestObject(HandlerContext context, CancellationToken cancellationToken)
        {
            var request = context.Request.RequestData.GetRequest();
            if (string.IsNullOrWhiteSpace(request))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(OAuth.ErrorMessages.MISSING_PARAMETER, BCAuthenticationRequestParameters.Request));
            }

            var validationResult = await _requestObjectValidator.Validate(request, (OpenIdClient)context.Client, cancellationToken, ErrorCodes.INVALID_REQUEST);
            var audiences = validationResult.JwsPayload.GetAudiences();
            var issuer = validationResult.JwsPayload.GetIssuer();
            var exp = validationResult.JwsPayload.GetExpirationTime();
            var iat = validationResult.JwsPayload.GetIat();
            var nbf = validationResult.JwsPayload.GetNbf();
            var jti = validationResult.JwsPayload.GetJti();
            if (audiences == null || !audiences.Any())
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.AUTH_REQUEST_NO_AUDIENCE);
            }

            if (!audiences.Contains(context.Request.IssuerName))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.AUTH_REQUEST_BAD_AUDIENCE);
            }

            if (string.IsNullOrWhiteSpace(issuer))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.AUTH_REQUEST_NO_ISSUER);
            }

            if (issuer != context.Client.ClientId)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.AUTH_REQUEST_BAD_ISSUER);
            }

            if (exp == default(double))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.AUTH_REQUEST_NO_EXPIRATION);
            }

            var expirationDateTime = exp.ConvertFromUnixTimestamp();
            var currentDateTime = DateTime.UtcNow;
            if (currentDateTime >= expirationDateTime)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.AUTH_REQUEST_IS_EXPIRED);
            }

            if (currentDateTime.AddSeconds(_options.MaxRequestLifetime) <= expirationDateTime)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.AUTH_REQUEST_MAXIMUM_LIFETIME, _options.MaxRequestLifetime));
            }

            if (iat == default(double))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.AUTH_REQUEST_NO_IAT);
            }

            if (nbf == default(double))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.AUTH_REQUEST_NO_NBF);
            }

            if (currentDateTime < nbf.ConvertFromUnixTimestamp())
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.AUTH_REQUEST_BAD_NBF, nbf.ConvertFromUnixTimestamp()));
            }

            var minusMaxRequestLifetime = -_options.MaxRequestLifetime;
            if (currentDateTime.AddSeconds(minusMaxRequestLifetime) > nbf.ConvertFromUnixTimestamp())
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.AUTH_REQUEST_MAXIMUM_LIFETIME, _options.MaxRequestLifetime));
            }

            if (string.IsNullOrWhiteSpace(jti))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.AUTH_REQUEST_NO_JTI);
            }

            var openidClient = (OpenIdClient)context.Client;
            if (openidClient.BCAuthenticationRequestSigningAlg != validationResult.JwsHeader.Alg)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.AUTH_REQUEST_ALG_NOT_VALID, openidClient.BCAuthenticationRequestSigningAlg));
            }

            context.Request.SetRequestData(JObject.FromObject(validationResult.JwsPayload));
        }

        private void CheckScopes(HandlerContext context)
        {
            var requestedScopes = context.Request.RequestData.GetScopesFromAuthorizationRequest();
            if (!requestedScopes.Any())
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(OAuth.ErrorMessages.MISSING_PARAMETER, OAuth.DTOs.AuthorizationRequestParameters.Scope));
            }

            ScopeHelper.Validate(requestedScopes, context.Client.AllowedScopes.Select(s => s.Name));
        }

        private void CheckClientNotificationToken(HandlerContext context)
        {
            var clientNotificationToken = context.Request.RequestData.GetClientNotificationToken();
            var openidClient = (OpenIdClient)context.Client;
            if (openidClient.BCTokenDeliveryMode != SIDOpenIdConstants.StandardNotificationModes.Ping && 
                openidClient.BCTokenDeliveryMode != SIDOpenIdConstants.StandardNotificationModes.Push)
            {
                return;
            }

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

        private async Task<OAuthUser> CheckLoginHintToken(HandlerContext context, CancellationToken cancellationToken)
        {
            var loginHintToken = context.Request.RequestData.GetLoginHintToken();
            if (!string.IsNullOrWhiteSpace(loginHintToken))
            {
                var payload = await ExtractHint(loginHintToken, cancellationToken);
                return await CheckHint(payload, cancellationToken);
            }

            return null;
        }

        private async Task<OAuthUser> CheckIdTokenHint(HandlerContext context, CancellationToken cancellationToken)
        {
            var idTokenHint = context.Request.RequestData.GetIdTokenHintFromAuthorizationRequest();
            if (!string.IsNullOrWhiteSpace(idTokenHint))
            {
                var payload = await ExtractHint(idTokenHint, cancellationToken);
                if (!payload.GetAudiences().Contains(context.Request.IssuerName))
                {
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_AUDIENCE_IDTOKENHINT);
                }

                return await CheckHint(payload, cancellationToken);
            }

            return null;
        }

        private async Task<OAuthUser> CheckLoginHint(HandlerContext context, CancellationToken cancellationToken)
        {
            var loginHint = context.Request.RequestData.GetLoginHintFromAuthorizationRequest();
            if (!string.IsNullOrEmpty(loginHint))
            {
                var user = await _oauthUserRepository.FindOAuthUserByClaim(Jwt.Constants.UserClaims.Subject, loginHint, cancellationToken);
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
            var bindingMessage = context.Request.RequestData.GetBindingMessage();
            if (!string.IsNullOrWhiteSpace(bindingMessage) && bindingMessage.Count() > _options.MaxBindingMessageSize)
            {
                throw new OAuthException(ErrorCodes.INVALID_BINDING_MESSAGE, string.Format(ErrorMessages.BINDING_MESSAGE_MUST_NOT_EXCEED, _options.MaxBindingMessageSize));
            }
        }

        private void CheckRequestedExpiry(HandlerContext context)
        {
            var requestedExpiry = context.Request.RequestData.GetRequestedExpiry();
            if (requestedExpiry.HasValue && requestedExpiry.Value < 0)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.REQUESTED_EXPIRY_MUST_BE_POSITIVE);
            }
        }

        private async Task<OAuthUser> CheckHint(JwsPayload jwsPayload, CancellationToken cancellationToken)
        {
            var exp = jwsPayload.GetExpirationTime();
            var currentDateTime = DateTime.UtcNow.ConvertToUnixTimestamp();
            if (currentDateTime > exp)
            {
                throw new OAuthException(ErrorCodes.EXPIRED_LOGIN_HINT_TOKEN, ErrorMessages.LOGIN_HINT_TOKEN_IS_EXPIRED);
            }

            var subject = jwsPayload.GetSub();
            var user = await _oauthUserRepository.FindOAuthUserByClaim(Jwt.Constants.UserClaims.Subject, subject, cancellationToken);
            if (user == null)
            {
                throw new OAuthException(ErrorCodes.UNKNOWN_USER_ID, string.Format(ErrorMessages.UNKNOWN_USER, subject));
            }

            return user;
        }

        protected async Task<JwsPayload> ExtractHint(string tokenHint, CancellationToken cancellationToken)
        {
            if (!_jwtParser.IsJwsToken(tokenHint) && !_jwtParser.IsJweToken(tokenHint))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_IDTOKENHINT);
            }

            if (_jwtParser.IsJweToken(tokenHint))
            {
                tokenHint = await _jwtParser.Decrypt(tokenHint, cancellationToken);
                if (string.IsNullOrWhiteSpace(tokenHint))
                {
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_IDTOKENHINT);
                }
            }

            return await _jwtParser.Unsign(tokenHint, cancellationToken);
        }
    }
}
