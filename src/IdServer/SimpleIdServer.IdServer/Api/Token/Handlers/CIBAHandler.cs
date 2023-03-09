// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Api.Token.TokenBuilders;
using SimpleIdServer.IdServer.Api.Token.TokenProfiles;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.Handlers
{
    public class CIBAHandler : BaseCredentialsHandler
    {
        private readonly ILogger<CIBAHandler> _logger;
        private readonly IUserRepository _userRepository;
        private readonly ICIBAGrantTypeValidator _cibaGrantTypeValidator;
        private readonly IEnumerable<ITokenBuilder> _tokenBuilders;
        private readonly IEnumerable<ITokenProfile> _tokenProfiles;
        private readonly IBCAuthorizeRepository _bcAuthorizeRepository;

        public CIBAHandler(
            ILogger<CIBAHandler> logger,
            IUserRepository userRepository,
            ICIBAGrantTypeValidator cibaGrantTypeValidator,
            IEnumerable<ITokenBuilder> tokenBuilders,
            IEnumerable<ITokenProfile> tokensProfiles,
            IClientAuthenticationHelper clientAuthenticationHelper,
            IOptions<IdServerHostOptions> options,
            IBCAuthorizeRepository bcAuthorizeRepository) : base(clientAuthenticationHelper, options)
        {
            _logger = logger;
            _userRepository = userRepository;
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
                var user = await _userRepository.Query().Include(u => u.OAuthUserClaims).Include(u => u.Realms).FirstOrDefaultAsync(u => u.Id == authRequest.UserId && u.Realms.Any(r => r.Name == context.Realm), cancellationToken);
                context.SetUser(user);
                foreach (var tokenBuilder in _tokenBuilders)
                    await tokenBuilder.Build(new BuildTokenParameter { Scopes = authRequest.Scopes }, context, cancellationToken);

                _tokenProfiles.First(t => t.Profile == (context.Client.PreferredTokenProfile ?? Options.DefaultTokenProfile)).Enrich(context);
                var result = BuildResult(context, authRequest.Scopes);
                foreach (var kvp in context.Response.Parameters)
                    result.Add(kvp.Key, kvp.Value);

                authRequest.Send();
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
            finally
            {
                await _bcAuthorizeRepository.SaveChanges(cancellationToken);
            }
        }
    }
}
