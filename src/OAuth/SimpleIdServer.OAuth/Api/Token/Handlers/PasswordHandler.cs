// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api.Token.Helpers;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.Api.Token.TokenProfiles;
using SimpleIdServer.OAuth.Api.Token.Validators;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Token.Handlers
{
    public class PasswordHandler : BaseCredentialsHandler
    {
        private readonly IPasswordGrantTypeValidator _passwordGrantTypeValidator;
        private readonly IOAuthUserQueryRepository _oauthUserQueryRepository;
        private readonly IEnumerable<ITokenProfile> _tokenProfiles;
        private readonly IEnumerable<ITokenBuilder> _tokenBuilders;

        public PasswordHandler(IPasswordGrantTypeValidator passwordGrantTypeValidator, IOAuthUserQueryRepository oauthUserQueryRepository, IEnumerable<ITokenProfile> tokenProfiles,
            IEnumerable<ITokenBuilder> tokenBuilders, IClientAuthenticationHelper clientAuthenticationHelper) : base(clientAuthenticationHelper)
        {
            _passwordGrantTypeValidator = passwordGrantTypeValidator;
            _oauthUserQueryRepository = oauthUserQueryRepository;
            _tokenProfiles = tokenProfiles;
            _tokenBuilders = tokenBuilders;
        }

        public override string GrantType => "password";

        public override async Task<JObject> Handle(HandlerContext context)
        {
            _passwordGrantTypeValidator.Validate(context);
            var oauthClient = await AuthenticateClient(context).ConfigureAwait(false);
            context.SetClient(oauthClient);
            var scopes = ScopeHelper.Validate(context.Request.HttpBody.GetStr(TokenRequestParameters.Scope), oauthClient.AllowedScopes.Select(s => s.Name));
            var userName = context.Request.HttpBody.GetStr(TokenRequestParameters.Username);
            var password = context.Request.HttpBody.GetStr(TokenRequestParameters.Password);
            var user = await _oauthUserQueryRepository.FindOAuthUserByLoginAndCredential(userName, "pwd", PasswordHelper.ComputeHash(password));
            if (user == null)
            {
                throw new OAuthException(ErrorCodes.INVALID_GRANT, ErrorMessages.BAD_USER_CREDENTIAL);
            }

            context.SetUser(user);
            var result = BuildResult(context, scopes);
            foreach (var tokenBuilder in _tokenBuilders)
            {
                await tokenBuilder.Build(scopes, context).ConfigureAwait(false);
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