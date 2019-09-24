// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api.Token.Helpers;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.Api.Token.TokenProfiles;
using SimpleIdServer.OAuth.Api.Token.Validators;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Helpers;
using System.Collections.Generic;
using System.Linq;
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

        public override async Task<JObject> Handle(HandlerContext context)
        {
            _authorizationCodeGrantTypeValidator.Validate(context);
            var oauthClient = await AuthenticateClient(context);
            context.SetClient(oauthClient);
            var code = context.Request.HttpBody.GetAuthorizationCode();
            var jwsPayload = _grantedTokenHelper.GetAuthorizationCode(code);
            if (jwsPayload == null)
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.BAD_AUTHORIZATION_CODE);
            }

            if (!jwsPayload.GetAudiences().Contains(oauthClient.ClientId))
            {
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.REFRESH_TOKEN_NOT_ISSUED_BY_CLIENT);
            }

            _grantedTokenHelper.RemoveAuthorizationCode(code);
            var scopes = jwsPayload.GetScopes();
            var result = BuildResult(context, scopes);
            foreach (var tokenBuilder in _tokenBuilders)
            {
                await tokenBuilder.Build(jwsPayload, context).ConfigureAwait(false);
            }

            _tokenProfiles.First(t => t.Profile == context.Client.PreferredTokenProfile).Enrich(context);
            foreach (var kvp in context.Response.Parameters)
            {
                result.Add(kvp.Key, kvp.Value);
            }

            return result;
        }
    }
}