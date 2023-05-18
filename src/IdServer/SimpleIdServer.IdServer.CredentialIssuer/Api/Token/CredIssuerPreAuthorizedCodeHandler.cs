// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Api.Token.TokenBuilders;
using SimpleIdServer.IdServer.Api.Token.TokenProfiles;
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
    public class CredIssuerPreAuthorizedCodeHandler : PreAuthorizedCodeHandler
    {
        private readonly CredentialIssuerOptions _options;
        private readonly ICredIssuerTokenHelper _tokenHelper;

        public CredIssuerPreAuthorizedCodeHandler(IOptions<CredentialIssuerOptions> opts, ICredIssuerTokenHelper tokenHelper, IPreAuthorizedCodeValidator validator, IBusControl busControl, IClientAuthenticationHelper clientAuthenticationHelper, IGrantedTokenHelper grantedTokenHelper, IEnumerable<ITokenProfile> tokenProfiles, IEnumerable<ITokenBuilder> tokenBuilders, IClientRepository clientRepository, IGrantHelper audienceHelper, IUserRepository userRepository, IOptions<IdServerHostOptions> options) : base(validator, busControl, clientAuthenticationHelper, grantedTokenHelper, tokenProfiles, tokenBuilders, clientRepository, audienceHelper, userRepository, options)
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
