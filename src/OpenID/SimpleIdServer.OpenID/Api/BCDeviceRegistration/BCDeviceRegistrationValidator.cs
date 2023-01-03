// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.Domains;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.Store;
using System;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.BCDeviceRegistration
{
    public interface IBCDeviceRegistrationValidator
    {
        Task<User> Validate(HandlerContext context, CancellationToken cancellationToken);
    }

    public class BCDeviceRegistrationValidator : IBCDeviceRegistrationValidator
    {
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IUserRepository _userRepository;

        public BCDeviceRegistrationValidator(IJwtBuilder jwtBuilder, IUserRepository userRepository)
        {
            _jwtBuilder = jwtBuilder;
            _userRepository = userRepository;
        }

        public async Task<User> Validate(HandlerContext context, CancellationToken cancellationToken)
        {
            var user = await ValidateIdTokenHint(context, cancellationToken);
            ValidateDeviceRegistrationToken(context);
            return user;
        }

        protected virtual async Task<User> ValidateIdTokenHint(HandlerContext context, CancellationToken cancellationToken)
        {
            var idTokenHint = context.Request.RequestData.GetIdTokenHintFromAuthorizationRequest();
            if (string.IsNullOrWhiteSpace(idTokenHint))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(OAuth.ErrorMessages.MISSING_PARAMETER, AuthorizationRequestParameters.IdTokenHint));

            var payload = ExtractHint(idTokenHint);
            if (!payload.Audiences.Contains(context.Request.IssuerName))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_AUDIENCE_IDTOKENHINT);

            return await CheckHint(payload, cancellationToken);
        }

        protected virtual void ValidateDeviceRegistrationToken(HandlerContext context)
        {
            var deviceRegistrationToken = context.Request.RequestData.GetDeviceRegistrationToken();
            if (string.IsNullOrWhiteSpace(deviceRegistrationToken))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(OAuth.ErrorMessages.MISSING_PARAMETER, BCDeviceRegistrationRequestParameters.DeviceRegistrationToken));
            }
        }

        protected virtual JsonWebToken ExtractHint(string tokenHint)
        {
            // TODO : Throw more exception.
            var result = _jwtBuilder.ReadSelfIssuedJsonWebToken(tokenHint);
            if(result.Error != null)
            {
                switch(result.Error.Value)
                {
                    case JsonWebTokenErrors.INVALID_JWT:
                    case JsonWebTokenErrors.UNKNOWN_JWK:
                    case JsonWebTokenErrors.BAD_SIGNATURE:
                    case JsonWebTokenErrors.CANNOT_BE_DECRYPTED:
                        throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_IDTOKENHINT);
                }
            }

            return result.Jwt;
        }

        protected virtual async Task<User> CheckHint(JsonWebToken jsonWebToken, CancellationToken cancellationToken)
        {
            var exp = jsonWebToken.ValidTo;
            if (DateTime.UtcNow > exp)
                throw new OAuthException(ErrorCodes.EXPIRED_LOGIN_HINT_TOKEN, ErrorMessages.LOGIN_HINT_TOKEN_IS_EXPIRED);

            var subject = jsonWebToken.Subject;
            var user = await _userRepository.Query().Include(u => u.Claims).FirstOrDefaultAsync(u => u.Claims.Any(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == subject), cancellationToken);
            if (user == null)
                throw new OAuthException(ErrorCodes.UNKNOWN_USER_ID, string.Format(ErrorMessages.UNKNOWN_USER, subject));

            return user;
        }
    }
}
