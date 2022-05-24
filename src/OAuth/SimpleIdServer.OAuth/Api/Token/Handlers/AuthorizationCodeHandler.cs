// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleIdServer.OAuth.Api.Token.Helpers;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.Api.Token.TokenProfiles;
using SimpleIdServer.OAuth.Api.Token.Validators;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Persistence.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Token.Handlers
{
    public class AuthorizationCodeHandler : BaseCredentialsHandler
    {
        private readonly IAuthorizationCodeGrantTypeValidator _authorizationCodeGrantTypeValidator;
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IEnumerable<ITokenProfile> _tokenProfiles;
        private readonly IEnumerable<ITokenBuilder> _tokenBuilders;
        private readonly ILogger<AuthorizationCodeHandler> _logger;

        public AuthorizationCodeHandler(
            IAuthorizationCodeGrantTypeValidator authorizationCodeGrantTypeValidator, 
            IGrantedTokenHelper grantedTokenHelper, 
            IEnumerable<ITokenProfile> tokenProfiles,
            IEnumerable<ITokenBuilder> tokenBuilders,
            IClientAuthenticationHelper clientAuthenticationHelper,
            ILogger<AuthorizationCodeHandler> logger) : base(clientAuthenticationHelper)
        {
            _authorizationCodeGrantTypeValidator = authorizationCodeGrantTypeValidator;
            _grantedTokenHelper = grantedTokenHelper;
            _tokenProfiles = tokenProfiles;
            _tokenBuilders = tokenBuilders;
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
                var previousRequest = await _grantedTokenHelper.GetAuthorizationCode(code, cancellationToken);
                if (previousRequest == null)
                {
                    // https://tools.ietf.org/html/rfc6749#section-4.1.2
                    var searchResult = await _grantedTokenHelper.SearchTokens(new SearchTokenParameter
                    {
                        AuthorizationCode = code
                    }, cancellationToken);
                    if (searchResult.Content.Any())
                    {
                        await _grantedTokenHelper.RemoveTokens(searchResult.Content, cancellationToken);
                        _logger.LogError($"authorization code '{code}' has already been used, all tokens previously issued have been revoked");
                        return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_GRANT, ErrorMessages.AUTHORIZATION_CODE_ALREADY_USED);
                    }

                    return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_GRANT, ErrorMessages.BAD_AUTHORIZATION_CODE);
                }

                var previousClientId = previousRequest.GetClientId();
                if (!previousClientId.Equals(oauthClient.ClientId, StringComparison.InvariantCultureIgnoreCase))
                {
                    return BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_GRANT, ErrorMessages.REFRESH_TOKEN_NOT_ISSUED_BY_CLIENT);
                }

                await _grantedTokenHelper.RemoveAuthorizationCode(code, cancellationToken);
                var scopes = previousRequest.GetScopesFromAuthorizationRequest();
                var result = BuildResult(context, scopes);
                foreach (var tokenBuilder in _tokenBuilders)
                {
                    await tokenBuilder.Refresh(previousRequest, context, cancellationToken);
                }

                _tokenProfiles.First(t => t.Profile == context.Client.PreferredTokenProfile).Enrich(context);
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
    }
}