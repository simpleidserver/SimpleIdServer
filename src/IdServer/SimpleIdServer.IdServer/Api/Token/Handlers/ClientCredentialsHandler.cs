// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Api.Token.TokenBuilders;
using SimpleIdServer.IdServer.Api.Token.TokenProfiles;
using SimpleIdServer.IdServer.Api.Token.Validators;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.Handlers
{
    public class ClientCredentialsHandler : BaseCredentialsHandler
    {
        private readonly IClientCredentialsGrantTypeValidator _clientCredentialsGrantTypeValidator;
        private readonly IEnumerable<ITokenProfile> _tokenProfiles;
        private readonly IEnumerable<ITokenBuilder> _tokenBuilders;
        private readonly IAudienceHelper _audienceHelper;
        private readonly IdServerHostOptions _options;

        public ClientCredentialsHandler(
            IClientCredentialsGrantTypeValidator clientCredentialsGrantTypeValidator, 
            IEnumerable<ITokenProfile> tokenProfiles,
            IEnumerable<ITokenBuilder> tokenBuilders, 
            IClientAuthenticationHelper clientAuthenticationHelper,
            IAudienceHelper audienceHelper,
            IOptions<IdServerHostOptions> options) : base(clientAuthenticationHelper, options)
        {
            _clientCredentialsGrantTypeValidator = clientCredentialsGrantTypeValidator;
            _tokenProfiles = tokenProfiles;
            _tokenBuilders = tokenBuilders;
            _audienceHelper = audienceHelper;
            _options = options.Value;
        }

        public const string GRANT_TYPE = "client_credentials";
        public override string GrantType { get => GRANT_TYPE; }

        public override async Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken)
        {
            try
            {
                _clientCredentialsGrantTypeValidator.Validate(context);
                var oauthClient = await AuthenticateClient(context, cancellationToken);
                context.SetClient(oauthClient);
                var scopes = ScopeHelper.Validate(context.Request.RequestData.GetStr(TokenRequestParameters.Scope), oauthClient.Scopes.Select(s => s.Name));
                var resources = context.Request.RequestData.GetResourcesFromAuthorizationRequest();
                var extractionResult = await _audienceHelper.Extract(context.Client.ClientId, scopes, resources, cancellationToken);
                var result = BuildResult(context, extractionResult.Scopes);
                foreach (var tokenBuilder in _tokenBuilders)
                    await tokenBuilder.Build(extractionResult.Scopes, extractionResult.Audiences, new List<AuthorizationRequestClaimParameter>(), context, cancellationToken);

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