// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Api.Token.TokenBuilders;
using SimpleIdServer.IdServer.Api.Token.TokenProfiles;
using SimpleIdServer.IdServer.Api.Token.Validators;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.Handlers
{
    public class AuthorizationCodeHandler : BaseCredentialsHandler
    {
        private readonly IAuthorizationCodeGrantTypeValidator _authorizationCodeGrantTypeValidator;
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IEnumerable<ITokenProfile> _tokenProfiles;
        private readonly IEnumerable<ITokenBuilder> _tokenBuilders;
        private readonly IUserRepository _userRepository;
        private readonly IAudienceHelper _audienceHelper;
        private readonly IdServerHostOptions _options;
        private readonly ILogger<AuthorizationCodeHandler> _logger;

        public AuthorizationCodeHandler(
            IAuthorizationCodeGrantTypeValidator authorizationCodeGrantTypeValidator, 
            IGrantedTokenHelper grantedTokenHelper, 
            IEnumerable<ITokenProfile> tokenProfiles,
            IEnumerable<ITokenBuilder> tokenBuilders,
            IUserRepository usrRepository,
            IClientAuthenticationHelper clientAuthenticationHelper,
            IAudienceHelper audienceHelper,
            IOptions<IdServerHostOptions> options,
            ILogger<AuthorizationCodeHandler> logger) : base(clientAuthenticationHelper, options)
        {
            _authorizationCodeGrantTypeValidator = authorizationCodeGrantTypeValidator;
            _grantedTokenHelper = grantedTokenHelper;
            _tokenProfiles = tokenProfiles;
            _tokenBuilders = tokenBuilders;
            _userRepository = usrRepository;
            _audienceHelper = audienceHelper;
            _options = options.Value;
            _logger = logger;
        }

        public override string GrantType => GRANT_TYPE;
        public static string GRANT_TYPE = "authorization_code";

        public override async Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken)
        {
            try
            {
                _authorizationCodeGrantTypeValidator.Validate(context);
                var oauthClient = await AuthenticateClient(context, cancellationToken);
                context.SetClient(oauthClient);
                var code = context.Request.RequestData.GetAuthorizationCode();
                var redirectUri = context.Request.RequestData.GetRedirectUri();
                var previousRequest = await _grantedTokenHelper.GetAuthorizationCode(code, cancellationToken);
                if (previousRequest == null)
                {
                    // https://tools.ietf.org/html/rfc6749#section-4.1.2
                    var searchResult = await _grantedTokenHelper.GetTokensByAuthorizationCode(code, cancellationToken);
                    if (searchResult.Any())
                    {
                        await _grantedTokenHelper.RemoveTokens(searchResult, cancellationToken);
                        _logger.LogError($"authorization code '{code}' has already been used, all tokens previously issued have been revoked");
                        return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_GRANT, ErrorMessages.AUTHORIZATION_CODE_ALREADY_USED);
                    }

                    return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_GRANT, ErrorMessages.BAD_AUTHORIZATION_CODE);
                }

                var previousClientId = previousRequest.GetClientId();
                var previousRedirectUrl = previousRequest.GetRedirectUri();
                var claims = previousRequest.GetClaimsFromAuthorizationRequest();
                if (!previousClientId.Equals(oauthClient.ClientId, StringComparison.InvariantCultureIgnoreCase)) return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_GRANT, ErrorMessages.AUTHORIZATION_CODE_NOT_ISSUED_BY_CLIENT);
                if (!previousRedirectUrl.Equals(redirectUri, StringComparison.InvariantCultureIgnoreCase)) return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_GRANT, ErrorMessages.NOT_SAME_REDIRECT_URI);
                await _grantedTokenHelper.RemoveAuthorizationCode(code, cancellationToken);

                var scopes = GetScopes(previousRequest, context);
                var resources = GetResources(previousRequest, context);
                var extractionResult = await _audienceHelper.Extract(context.Client.ClientId, scopes, resources, cancellationToken);
                var result = BuildResult(context, extractionResult.Scopes);
                await Authenticate(previousRequest, context, cancellationToken);
                context.SetOriginalRequest(previousRequest);
                foreach (var tokenBuilder in _tokenBuilders)
                    await tokenBuilder.Build(extractionResult.Scopes, extractionResult.Audiences, claims, context, cancellationToken);

                _tokenProfiles.First(t => t.Profile == (context.Client.PreferredTokenProfile ?? _options.DefaultTokenProfile)).Enrich(context);
                foreach (var kvp in context.Response.Parameters)
                {
                    result.Add(kvp.Key, kvp.Value);
                }

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

        async Task Authenticate(JsonObject previousQueryParameters, HandlerContext handlerContext, CancellationToken token)
        {
            if (!previousQueryParameters.ContainsKey(JwtRegisteredClaimNames.Sub))
                return;
            handlerContext.SetUser(await _userRepository.Query().Include(u => u.OAuthUserClaims).FirstOrDefaultAsync(u => u.Id == previousQueryParameters[JwtRegisteredClaimNames.Sub].GetValue<string>(), token));
        }
    }
}