// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.OAuth.Api.Token.Helpers;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.Api.Token.TokenProfiles;
using SimpleIdServer.OAuth.Api.Token.Validators;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Token.Handlers
{
    public class AuthorizationCodeHandler : BaseCredentialsHandler
    {
        private readonly IAuthorizationCodeGrantTypeValidator _authorizationCodeGrantTypeValidator;
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IEnumerable<ITokenProfile> _tokenProfiles;
        private readonly IEnumerable<ITokenBuilder> _tokenBuilders;

        public AuthorizationCodeHandler(IAuthorizationCodeGrantTypeValidator authorizationCodeGrantTypeValidator, IGrantedTokenHelper grantedTokenHelper, IEnumerable<ITokenProfile> tokenProfiles,
            IEnumerable<ITokenBuilder> tokenBuilders, IClientAuthenticationHelper clientAuthenticationHelper) : base(clientAuthenticationHelper)
        {
            _authorizationCodeGrantTypeValidator = authorizationCodeGrantTypeValidator;
            _grantedTokenHelper = grantedTokenHelper;
            _tokenProfiles = tokenProfiles;
            _tokenBuilders = tokenBuilders;
        }

        public override string GrantType => GRANT_TYPE;
        public static string GRANT_TYPE = "authorization_code";

        public override async Task<IActionResult> Handle(HandlerContext context)
        {
            try
            {
                _authorizationCodeGrantTypeValidator.Validate(context);
                var oauthClient = await AuthenticateClient(context);
                context.SetClient(oauthClient);
                var code = context.Request.HttpBody.GetAuthorizationCode();
                var previousRequest = _grantedTokenHelper.GetAuthorizationCode(code);
                if (previousRequest == null)
                {
                    return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.BAD_AUTHORIZATION_CODE);
                }

                var previousClientId = previousRequest.GetClientId();
                if (!previousClientId.Equals(oauthClient.ClientId, StringComparison.InvariantCultureIgnoreCase))
                {
                    return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.REFRESH_TOKEN_NOT_ISSUED_BY_CLIENT);
                }

                _grantedTokenHelper.RemoveAuthorizationCode(code);
                var scopes = previousRequest.GetScopesFromAuthorizationRequest();
                var result = BuildResult(context, scopes);
                foreach (var tokenBuilder in _tokenBuilders)
                {
                    await tokenBuilder.Refresh(previousRequest, context).ConfigureAwait(false);
                }

                _tokenProfiles.First(t => t.Profile == context.Client.PreferredTokenProfile).Enrich(context);
                foreach (var kvp in context.Response.Parameters)
                {
                    result.Add(kvp.Key, kvp.Value);
                }

                return new OkObjectResult(result);
            }
            catch (OAuthException ex)
            {
                return BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }
    }
}