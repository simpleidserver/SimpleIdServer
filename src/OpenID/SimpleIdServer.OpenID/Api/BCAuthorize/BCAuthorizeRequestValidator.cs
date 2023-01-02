// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.Domains;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Token.Helpers;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.OpenID.Options;
using SimpleIdServer.OpenID.Persistence;
using SimpleIdServer.Store;
using System;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.BCAuthorize
{
    public interface IBCAuthorizeRequestValidator
    {
        Task<User> ValidateCreate(HandlerContext context, CancellationToken cancellationToken);
        Task<BCAcceptRequestValidationResult> ValidateConfirm(HandlerContext context, CancellationToken cancellationToken);
        Task<Domains.BCAuthorize> ValidateReject(HandlerContext context, CancellationToken cancellationToken);
    }

    public class BCAuthorizeRequestValidator: IBCAuthorizeRequestValidator
    {
        private readonly IUserRepository _userRepository;
        private readonly IBCAuthorizeRepository _bcAuthorizeRepository;
        private readonly IRequestObjectValidator _requestObjectValidator;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly OpenIDHostOptions _options;

        public BCAuthorizeRequestValidator(
            IUserRepository userRepository, 
            IBCAuthorizeRepository bcAuthorizeRepository,
            IRequestObjectValidator requestObjectValidator,
            IJwtBuilder jwtBuilder,
            IOptions<OpenIDHostOptions> options)
        {
            _userRepository = userRepository;
            _bcAuthorizeRepository = bcAuthorizeRepository;
            _requestObjectValidator = requestObjectValidator;
            _jwtBuilder = jwtBuilder;
            _options = options.Value;
        }

        public virtual async Task<User> ValidateCreate(HandlerContext context, CancellationToken cancellationToken)
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

            var jsonWebTokenResult = await _jwtBuilder.ReadJsonWebToken(request, context.Client, cancellationToken);
            if(jsonWebTokenResult.Error != null)
                switch(jsonWebTokenResult.Error.Value)
                {
                    case JsonWebTokenErrors.INVALID_JWT:
                        throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_REQUEST_PARAMETER);
                    case JsonWebTokenErrors.CANNOT_BE_DECRYPTED:
                        throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_JWE_REQUEST_PARAMETER);
                    case JsonWebTokenErrors.UNKNOWN_JWK:
                        throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.UNKNOWN_JSON_WEBKEY);
                    case JsonWebTokenErrors.BAD_SIGNATURE:
                        throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_JWS_REQUEST_PARAMETER);
                }

            var audiences = jsonWebTokenResult.Jwt.Audiences;
            var issuer = jsonWebTokenResult.Jwt.Issuer;
            var exp = jsonWebTokenResult.Jwt.ValidTo;
            var iat = jsonWebTokenResult.Jwt.IssuedAt;
            var nbf = jsonWebTokenResult.Jwt.ValidFrom;
            var jti = jsonWebTokenResult.Jwt.Id;
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

            if (exp == default(DateTime))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.AUTH_REQUEST_NO_EXPIRATION);
            }

            var currentDateTime = DateTime.UtcNow;
            if (currentDateTime >= exp)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.AUTH_REQUEST_IS_EXPIRED);
            }

            if (currentDateTime.AddSeconds(_options.MaxRequestLifetime) <= exp)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.AUTH_REQUEST_MAXIMUM_LIFETIME, _options.MaxRequestLifetime));
            }

            if (iat == default(DateTime))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.AUTH_REQUEST_NO_IAT);
            }

            if (nbf == default(DateTime))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.AUTH_REQUEST_NO_NBF);
            }

            if (currentDateTime < nbf)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.AUTH_REQUEST_BAD_NBF, nbf));
            }

            var minusMaxRequestLifetime = -_options.MaxRequestLifetime;
            if (currentDateTime.AddSeconds(minusMaxRequestLifetime) > nbf)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.AUTH_REQUEST_MAXIMUM_LIFETIME, _options.MaxRequestLifetime));
            }

            if (string.IsNullOrWhiteSpace(jti))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.AUTH_REQUEST_NO_JTI);
            }

            Client openidClient = context.Client;
            if (openidClient.GetBCAuthenticationRequestSigningAlg() != jsonWebTokenResult.Jwt.Alg)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.AUTH_REQUEST_ALG_NOT_VALID, openidClient.GetBCAuthenticationRequestSigningAlg()));
            }

            context.Request.SetRequestData(jsonWebTokenResult.Jwt.GetClaimJson());
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
            Client openidClient = context.Client;
            if (openidClient.GetBCTokenDeliveryMode() != SIDOpenIdConstants.StandardNotificationModes.Ping && 
                openidClient.GetBCTokenDeliveryMode() != SIDOpenIdConstants.StandardNotificationModes.Push)
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

            if (ASCII.GetByteCount(clientNotificationToken) * 8 < 128)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.CLIENT_NOTIFICATION_TOKEN_MUST_CONTAIN_AT_LEAST_128_BYTES);
            }
        }

        private async Task<User> CheckLoginHintToken(HandlerContext context, CancellationToken cancellationToken)
        {
            var loginHintToken = context.Request.RequestData.GetLoginHintToken();
            if (!string.IsNullOrWhiteSpace(loginHintToken))
            {
                var payload = ExtractHint(loginHintToken);
                return await CheckHint(payload, cancellationToken);
            }

            return null;
        }

        private async Task<User> CheckIdTokenHint(HandlerContext context, CancellationToken cancellationToken)
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

        private async Task<User> CheckLoginHint(HandlerContext context, CancellationToken cancellationToken)
        {
            var loginHint = context.Request.RequestData.GetLoginHintFromAuthorizationRequest();
            if (!string.IsNullOrEmpty(loginHint))
            {
                var user = await _userRepository.Query().Include(u => u.Claims).AsNoTracking().FirstOrDefaultAsync(u => u.Claims.Any(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == loginHint), cancellationToken);
                if (user == null)
                    throw new OAuthException(ErrorCodes.UNKNOWN_USER_ID, string.Format(ErrorMessages.UNKNOWN_USER, loginHint));

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

        private async Task<User> CheckHint(JsonWebToken jwsPayload, CancellationToken cancellationToken)
        {
            var exp = jwsPayload.ValidTo;
            var currentDateTime = DateTime.UtcNow;
            if (currentDateTime > exp)
                throw new OAuthException(ErrorCodes.EXPIRED_LOGIN_HINT_TOKEN, ErrorMessages.LOGIN_HINT_TOKEN_IS_EXPIRED);

            var subject = jwsPayload.Subject;
            var user = await _userRepository.Query().Include(u => u.Claims).AsNoTracking().FirstOrDefaultAsync(u => u.Claims.Any(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == subject), cancellationToken);
            if (user == null)
                throw new OAuthException(ErrorCodes.UNKNOWN_USER_ID, string.Format(ErrorMessages.UNKNOWN_USER, subject));

            return user;
        }

        protected JsonWebToken ExtractHint(string tokenHint, CancellationToken cancellationToken)
        {
            // TODO : Add more exception.
            var extractionResult = _jwtBuilder.ReadSelfIssuedJsonWebToken(tokenHint);
            if (extractionResult.Error != null)
                switch (extractionResult.Error.Value)
                {
                    case JsonWebTokenErrors.INVALID_JWT:
                    case JsonWebTokenErrors.CANNOT_BE_DECRYPTED:
                    case JsonWebTokenErrors.UNKNOWN_JWK:
                    case JsonWebTokenErrors.BAD_SIGNATURE:
                        throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_IDTOKENHINT);
                }

            return extractionResult.Jwt;
        }
    }
}
