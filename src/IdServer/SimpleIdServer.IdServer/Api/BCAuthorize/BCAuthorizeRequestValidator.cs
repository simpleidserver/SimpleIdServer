// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using System;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.BCAuthorize
{
    public interface IBCAuthorizeRequestValidator
    {
        Task<User> ValidateCreate(HandlerContext context, CancellationToken cancellationToken);
    }

    public class BCAcceptRequestValidationResult
    {
        public BCAcceptRequestValidationResult(User user, Domains.BCAuthorize authorize)
        {
            User = user;
            Authorize = authorize;
        }

        public User User { get; set; }
        public Domains.BCAuthorize Authorize { get; set; }
    }

    public class BCAuthorizeRequestValidator : IBCAuthorizeRequestValidator
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IdServerHostOptions _options;

        public BCAuthorizeRequestValidator(
            IUserRepository userRepository,
            IJwtBuilder jwtBuilder,
            IOptions<IdServerHostOptions> options)
        {
            _userRepository = userRepository;
            _jwtBuilder = jwtBuilder;
            _options = options.Value;
        }

        public async Task<User> ValidateCreate(HandlerContext context, CancellationToken cancellationToken)
        {
            await CheckRequestObject(context, cancellationToken);
            var tokens = new bool[]
            {
                string.IsNullOrWhiteSpace(context.Request.RequestData.GetLoginHintToken()),
                string.IsNullOrWhiteSpace(context.Request.RequestData.GetIdTokenHintFromAuthorizationRequest()),
                string.IsNullOrWhiteSpace(context.Request.RequestData.GetLoginHintFromAuthorizationRequest())
            };
            if (tokens.All(_ => _) || (tokens.Where(_ => !_).Count() > 1))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.ONE_HINT_MUST_BE_PASSED);

            var userCode = context.Request.RequestData.GetUserCode();
            if (context.Client.BCUserCodeParameter && string.IsNullOrWhiteSpace(userCode))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.MISSING_USER_CODE);

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
            CheckUserCode(user, userCode);
            return user;
        }

        /// <summary>
        /// Authorization server verifies the expiration.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected virtual async Task<User> CheckLoginHintToken(HandlerContext context, CancellationToken cancellationToken)
        {
            var loginHintToken = context.Request.RequestData.GetLoginHintToken();
            if (!string.IsNullOrWhiteSpace(loginHintToken))
            {
                var handler = new JsonWebTokenHandler();
                var payload = handler.ReadJsonWebToken(loginHintToken);
                return await CheckHint(context.Realm, payload, cancellationToken);
            }

            return null;
        }

        /// <summary>
        /// Value contains the subject of the user.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="OAuthException"></exception>
        protected virtual async Task<User> CheckLoginHint(HandlerContext context, CancellationToken cancellationToken)
        {
            var loginHint = context.Request.RequestData.GetLoginHintFromAuthorizationRequest();
            if (!string.IsNullOrEmpty(loginHint))
            {
                var user = await _userRepository.Query().Include(u => u.Groups).Include(u => u.Credentials).Include(u => u.Realms).AsNoTracking().FirstOrDefaultAsync(u => u.Name == loginHint && u.Realms.Any(r => r.RealmsName == context.Realm), cancellationToken);
                if (user == null)
                    throw new OAuthException(ErrorCodes.UNKNOWN_USER_ID, string.Format(ErrorMessages.UNKNOWN_USER, loginHint));

                return user;
            }

            return null;
        }

        protected virtual async Task<User> CheckHint(string realm, JsonWebToken jwsPayload, CancellationToken cancellationToken)
        {
            var exp = jwsPayload.ValidTo;
            var currentDateTime = DateTime.UtcNow;
            if (currentDateTime > exp)
                throw new OAuthException(ErrorCodes.EXPIRED_LOGIN_HINT_TOKEN, ErrorMessages.LOGIN_HINT_TOKEN_IS_EXPIRED);

            var subject = jwsPayload.Subject;
            var user = await _userRepository.Query().Include(u => u.Groups).Include(u => u.Credentials).Include(u => u.Realms).AsNoTracking().FirstOrDefaultAsync(u => u.Name == subject && u.Realms.Any(r => r.RealmsName == realm), cancellationToken);
            if (user == null)
                throw new OAuthException(ErrorCodes.UNKNOWN_USER_ID, string.Format(ErrorMessages.UNKNOWN_USER, subject));

            return user;
        }

        protected virtual void CheckUserCode(User user, string userCode)
        {
            if (string.IsNullOrWhiteSpace(userCode)) return;
            var credential = user.Credentials.FirstOrDefault(c => c.CredentialType == Constants.Areas.Password && c.IsActive);
            var hash = PasswordHelper.ComputeHash(userCode);
            if (credential == null || credential.Value != PasswordHelper.ComputeHash(userCode))
                throw new OAuthException(ErrorCodes.INVALID_CREDENTIALS, ErrorMessages.INVALID_USER_CODE);
        }

        private async Task CheckRequestObject(HandlerContext context, CancellationToken cancellationToken)
        {
            var request = context.Request.RequestData.GetRequest();
            if (string.IsNullOrWhiteSpace(request))
                return;

            var jsonWebTokenResult = await _jwtBuilder.ReadJsonWebToken(context.Realm, request, context.Client, context.Client.BCAuthenticationRequestSigningAlg, null, cancellationToken);
            if (jsonWebTokenResult.Error != null)
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, jsonWebTokenResult.Error);

            var audiences = jsonWebTokenResult.Jwt.Audiences;
            var issuer = jsonWebTokenResult.Jwt.Issuer;
            var exp = jsonWebTokenResult.Jwt.ValidTo;
            var nbf = jsonWebTokenResult.Jwt.ValidFrom;
            var jti = jsonWebTokenResult.Jwt.Id;
            if (audiences == null || !audiences.Any())
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.AUTH_REQUEST_NO_AUDIENCE);

            if (!audiences.Contains(context.GetIssuer()))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.AUTH_REQUEST_BAD_AUDIENCE);

            if (string.IsNullOrWhiteSpace(issuer))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.AUTH_REQUEST_NO_ISSUER);

            if (issuer != context.Client.ClientId)
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.AUTH_REQUEST_BAD_ISSUER);

            var currentDateTime = DateTime.UtcNow;
            if (currentDateTime >= exp)
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.AUTH_REQUEST_IS_EXPIRED);

            var diffSeconds = (exp - nbf).TotalSeconds;
            if (diffSeconds > _options.MaxRequestLifetime)
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.AUTH_REQUEST_MAXIMUM_LIFETIME, _options.MaxRequestLifetime));

            if (currentDateTime < nbf)
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.AUTH_REQUEST_BAD_NBF, nbf));

            if (string.IsNullOrWhiteSpace(jti))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.AUTH_REQUEST_NO_JTI);
            }

            Client openidClient = context.Client;
            if (openidClient.BCAuthenticationRequestSigningAlg != jsonWebTokenResult.Jwt.Alg)
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.AUTH_REQUEST_ALG_NOT_VALID, openidClient.BCAuthenticationRequestSigningAlg));

            context.Request.SetRequestData(jsonWebTokenResult.Jwt.GetClaimJson());
        }

        private void CheckScopes(HandlerContext context)
        {
            var requestedScopes = context.Request.RequestData.GetScopesFromAuthorizationRequest();
            var authDetails = context.Request.RequestData.GetAuthorizationDetailsFromAuthorizationRequest();
            if (!requestedScopes.Any() && !authDetails.Any())
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETERS, $"{AuthorizationRequestParameters.Scope},{AuthorizationRequestParameters.AuthorizationDetails}"));

            if(requestedScopes.Any()) ScopeHelper.Validate(requestedScopes, context.Client.Scopes.Select(s => s.Name));
            if(authDetails.Any())
            {
                if (authDetails != null && authDetails.Any(d => string.IsNullOrWhiteSpace(d.Type)))
                    throw new OAuthException(ErrorCodes.INVALID_AUTHORIZATION_DETAILS, ErrorMessages.AUTHORIZATION_DETAILS_TYPE_REQUIRED);
                var unsupportedAuthorizationDetailsTypes = authDetails.Where(d => !context.Client.AuthorizationDataTypes.Contains(d.Type));
                if (unsupportedAuthorizationDetailsTypes.Any())
                    throw new OAuthException(ErrorCodes.INVALID_AUTHORIZATION_DETAILS, string.Format(ErrorMessages.UNSUPPORTED_AUTHORIZATION_DETAILS_TYPES, string.Join(",", unsupportedAuthorizationDetailsTypes.Select(t => t.Type))));
            }
        }

        private void CheckClientNotificationToken(HandlerContext context)
        {
            var clientNotificationToken = context.Request.RequestData.GetClientNotificationToken();
            Client openidClient = context.Client;
            if (openidClient.BCTokenDeliveryMode != Constants.StandardNotificationModes.Ping &&
                openidClient.BCTokenDeliveryMode != Constants.StandardNotificationModes.Push)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(clientNotificationToken))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, DTOs.BCAuthenticationRequestParameters.ClientNotificationToken));

            if (clientNotificationToken.Length > 1024)
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.CLIENT_NOTIFICATION_TOKEN_MUST_NOT_EXCEED_1024);

            if (System.Text.Encoding.ASCII.GetByteCount(clientNotificationToken) * 8 < 128)
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.CLIENT_NOTIFICATION_TOKEN_MUST_CONTAIN_AT_LEAST_128_BYTES);
        }

        /// <summary>
        /// ID Token previously issued to the client by the OpenID Provider being passed back as a hint.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="OAuthException"></exception>
        private async Task<User> CheckIdTokenHint(HandlerContext context, CancellationToken cancellationToken)
        {
            var idTokenHint = context.Request.RequestData.GetIdTokenHintFromAuthorizationRequest();
            if (!string.IsNullOrWhiteSpace(idTokenHint))
            {
                var extractionResult = _jwtBuilder.ReadSelfIssuedJsonWebToken(context.Realm, idTokenHint);
                if (extractionResult.Error != null)
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, extractionResult.Error);
                var payload = extractionResult.Jwt;
                if (!payload.Audiences.Contains(context.GetIssuer()))
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_AUDIENCE_IDTOKENHINT);

                return await CheckHint(context.Realm, payload, cancellationToken);
            }

            return null;
        }

        private void CheckBindingMessage(HandlerContext context)
        {
            var bindingMessage = context.Request.RequestData.GetBindingMessage();
            if (!string.IsNullOrWhiteSpace(bindingMessage) && bindingMessage.Count() > _options.MaxBindingMessageSize)
                throw new OAuthException(ErrorCodes.INVALID_BINDING_MESSAGE, string.Format(ErrorMessages.BINDING_MESSAGE_MUST_NOT_EXCEED, _options.MaxBindingMessageSize));
        }

        private void CheckRequestedExpiry(HandlerContext context)
        {
            var requestedExpiry = context.Request.RequestData.GetRequestedExpiry();
            if (requestedExpiry.HasValue && requestedExpiry.Value < 0)
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.REQUESTED_EXPIRY_MUST_BE_POSITIVE);
        }
    }
}
