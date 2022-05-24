// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Api.Token.Helpers;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.Api.Token.TokenProfiles;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.Token.Handlers
{
    public class CIBAHandler : BaseCredentialsHandler
    {
        private readonly ILogger<CIBAHandler> _logger;
        private readonly IOAuthUserRepository _oauthUserRepository;
        private readonly ICIBAGrantTypeValidator _cibaGrantTypeValidator;
        private readonly IEnumerable<ITokenBuilder> _tokenBuilders;
        private readonly IEnumerable<ITokenProfile> _tokenProfiles;
        private readonly IBCAuthorizeRepository _bcAuthorizeRepository;

        public CIBAHandler(
            ILogger<CIBAHandler> logger,
            IOAuthUserRepository oauthUserQueryRepository,
            ICIBAGrantTypeValidator cibaGrantTypeValidator,
            IEnumerable<ITokenBuilder> tokenBuilders,
            IEnumerable<ITokenProfile> tokensProfiles,
            IClientAuthenticationHelper clientAuthenticationHelper,
            IBCAuthorizeRepository bcAuthorizeRepository) : base(clientAuthenticationHelper)
        {
            _logger = logger;
            _oauthUserRepository = oauthUserQueryRepository;
            _cibaGrantTypeValidator = cibaGrantTypeValidator;
            _tokenBuilders = tokenBuilders;
            _tokenProfiles = tokensProfiles;
            _bcAuthorizeRepository = bcAuthorizeRepository;
        }

        public const string GRANT_TYPE = "urn:openid:params:grant-type:ciba";
        public override string GrantType => GRANT_TYPE;

        public override async Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken)
        {
            try
            {
                var oauthClient = await AuthenticateClient(context, cancellationToken);
                context.SetClient(oauthClient);
                var authRequest = await _cibaGrantTypeValidator.Validate(context, cancellationToken);
                var user = await _oauthUserRepository.FindOAuthUserByLogin(authRequest.UserId, cancellationToken);
                context.SetUser(user);
                foreach (var tokenBuilder in _tokenBuilders)
                {
                    await tokenBuilder.Build(authRequest.Scopes, context, cancellationToken);
                }

                _tokenProfiles.First(t => t.Profile == context.Client.PreferredTokenProfile).Enrich(context);
                var result = BuildResult(context, authRequest.Scopes);
                foreach (var kvp in context.Response.Parameters)
                {
                    result.Add(kvp.Key, kvp.Value);
                }

                authRequest.Send();
                await _bcAuthorizeRepository.Update(authRequest, cancellationToken);
                await _bcAuthorizeRepository.SaveChanges(cancellationToken);
                return new OkObjectResult(result);
            }
            catch (OAuthUnauthorizedException ex)
            {
                return BuildError(HttpStatusCode.Unauthorized, ex.Code, ex.Message);
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                return BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }
    }
}
