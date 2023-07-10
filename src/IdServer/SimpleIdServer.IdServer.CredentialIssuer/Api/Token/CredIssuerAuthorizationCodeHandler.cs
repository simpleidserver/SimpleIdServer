// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Api.Token.TokenBuilders;
using SimpleIdServer.IdServer.Api.Token.TokenProfiles;
using SimpleIdServer.IdServer.Api.Token.Validators;
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using SimpleIdServer.IdServer.CredentialIssuer.Helpers;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.CredentialIssuer.Api.Token
{
    public class CredIssuerAuthorizationCodeHandler : AuthorizationCodeHandler
    {
        private readonly CredentialIssuerOptions _options;
        private readonly ICredIssuerTokenHelper _tokenHelper;

        public CredIssuerAuthorizationCodeHandler(IOptions<CredentialIssuerOptions> opts, ICredIssuerTokenHelper tokenHelper, IGrantedTokenHelper grantedTokenHelper, IAuthorizationCodeGrantTypeValidator authorizationCodeGrantTypeValidator, IEnumerable<ITokenProfile> tokenProfiles, IEnumerable<ITokenBuilder> tokenBuilders, IUserRepository usrRepository, IClientAuthenticationHelper clientAuthenticationHelper, IGrantHelper audienceHelper, IBusControl busControl, IDPOPProofValidator dpopProofValidator, IOptions<IdServerHostOptions> options, ILogger<AuthorizationCodeHandler> logger) : base(grantedTokenHelper, authorizationCodeGrantTypeValidator, tokenProfiles, tokenBuilders, usrRepository, clientAuthenticationHelper, audienceHelper, busControl, dpopProofValidator, options, logger)
        {
            _options = opts.Value;
            _tokenHelper = tokenHelper;
        }

        protected override async Task Enrich(HandlerContext handlerContext, JsonObject result, CancellationToken cancellationToken)
        {
            var expiresIn = handlerContext.Client.CNonceExpirationTimeInSeconds ?? _options.DefaultCNonceExpirationTimeInSeconds.Value;
            var credentialNonce = Guid.NewGuid().ToString();
            await _tokenHelper.AddCredentialNonce(credentialNonce, expiresIn, cancellationToken);
            result.Add(CredIssuerTokenResponseParameters.CNonce, credentialNonce);
            result.Add(CredIssuerTokenResponseParameters.CNonceExpiresIn, expiresIn);
        }
    }
}
