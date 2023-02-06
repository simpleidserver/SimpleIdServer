// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Api.Token.TokenBuilders;
using SimpleIdServer.IdServer.Api.Token.TokenProfiles;
using SimpleIdServer.IdServer.Api.Token.Validators;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.Handlers
{
    public class PasswordHandler : BaseCredentialsHandler
    {
        private readonly IPasswordGrantTypeValidator _passwordGrantTypeValidator;
        private readonly IUserRepository _userRepository;
        private readonly IEnumerable<ITokenProfile> _tokenProfiles;
        private readonly IEnumerable<ITokenBuilder> _tokenBuilders;
        private readonly IGrantHelper _audienceHelper;
        private readonly IAuthenticationHelper _userHelper;
        private readonly IdServerHostOptions _options;

        public PasswordHandler(
            IPasswordGrantTypeValidator passwordGrantTypeValidator, 
            IUserRepository userRepository, 
            IEnumerable<ITokenProfile> tokenProfiles,
            IEnumerable<ITokenBuilder> tokenBuilders, 
            IClientAuthenticationHelper clientAuthenticationHelper,
            IGrantHelper audienceHelper,
            IAuthenticationHelper userHelper,
            IOptions<IdServerHostOptions> options) : base(clientAuthenticationHelper, options)
        {
            _passwordGrantTypeValidator = passwordGrantTypeValidator;
            _userRepository = userRepository;
            _tokenProfiles = tokenProfiles;
            _tokenBuilders = tokenBuilders;
            _audienceHelper = audienceHelper;
            _userHelper = userHelper;
            _options = options.Value;
        }

        public const string GRANT_TYPE = "password";
        public override string GrantType => GRANT_TYPE;

        public override async Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken)
        {
            try
            {
                _passwordGrantTypeValidator.Validate(context);
                var oauthClient = await AuthenticateClient(context, cancellationToken);
                context.SetClient(oauthClient);
                var scopes = ScopeHelper.Validate(context.Request.RequestData.GetStr(TokenRequestParameters.Scope), oauthClient.Scopes.Select(s => s.Name));
                var resources = context.Request.RequestData.GetResourcesFromAuthorizationRequest();
                var extractionResult = await _audienceHelper.Extract(scopes, resources, cancellationToken);
                var userName = context.Request.RequestData.GetStr(TokenRequestParameters.Username);
                var password = context.Request.RequestData.GetStr(TokenRequestParameters.Password);
                var user = await _userHelper.FilterUsersByLogin(_userRepository.Query().Include(u=> u.Credentials).Include(u => u.OAuthUserClaims).AsNoTracking(), userName).FirstOrDefaultAsync(u => u.Credentials.Any(c => c.CredentialType == UserCredential.PWD && c.Value == PasswordHelper.ComputeHash(password) && c.IsActive), cancellationToken);
                if (user == null) return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_GRANT, ErrorMessages.BAD_USER_CREDENTIAL);
                context.SetUser(user);
                var result = BuildResult(context, extractionResult.Scopes);
                foreach (var tokenBuilder in _tokenBuilders)
                    await tokenBuilder.Build(new BuildTokenParameter { Scopes = extractionResult.Scopes, Audiences = extractionResult.Audiences }, context, cancellationToken);

                _tokenProfiles.First(t => t.Profile == (context.Client.PreferredTokenProfile ?? _options.DefaultTokenProfile)).Enrich(context);
                foreach (var kvp in context.Response.Parameters)    
                    result.Add(kvp.Key, kvp.Value);

                return new OkObjectResult(result);
            }
            catch (OAuthUnauthorizedException ex)
            {
                return BuildError(HttpStatusCode.Unauthorized, ex.Code, ex.Message);
            }
            catch (OAuthException ex)
            {
                return BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }
    }
}