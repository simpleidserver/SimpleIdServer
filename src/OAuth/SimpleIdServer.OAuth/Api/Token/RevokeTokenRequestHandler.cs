// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api.Token.Helpers;
using SimpleIdServer.OAuth.Api.Token.Validators;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Helpers;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Token
{
    public interface IRevokeTokenRequestHandler
    {
        Task Handle(JObject jObjHeader, JObject jObjBody, string issuerName);
    }

    public class RevokeTokenRequestHandler : IRevokeTokenRequestHandler
    {
        private readonly IRevokeTokenValidator _revokeTokenValidator;
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IClientAuthenticationHelper _clientAuthenticationHelper;

        public RevokeTokenRequestHandler(IRevokeTokenValidator revokeTokenValidator, IGrantedTokenHelper grantedTokenHelper, IClientAuthenticationHelper clientAuthenticationHelper)
        {
            _revokeTokenValidator = revokeTokenValidator;
            _grantedTokenHelper = grantedTokenHelper;
            _clientAuthenticationHelper = clientAuthenticationHelper;
        }

        public async Task Handle(JObject jObjHeader, JObject jObjBody, string issuerName)
        {
            _revokeTokenValidator.Validate(jObjBody);
            var oauthClient = await _clientAuthenticationHelper.AuthenticateClient(jObjBody, jObjBody, issuerName).ConfigureAwait(false);
            var refreshToken = _grantedTokenHelper.GetRefreshToken(jObjBody.GetStr(RevokeTokenRequestParameters.Token));
            if (refreshToken != null && !refreshToken.GetAudiences().Contains(oauthClient.ClientId))
            {
                throw new OAuthException(ErrorCodes.INVALID_CLIENT, ErrorMessages.UNAUTHORIZED_CLIENT);
            }

            _grantedTokenHelper.RemoveRefreshToken(jObjBody.GetStr(RevokeTokenRequestParameters.Token));
        }
    }
}