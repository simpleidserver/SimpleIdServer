// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.EntityFrameworkCore;
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
        private readonly ICredentialOfferRepository _credentialOfferRepository;
        private readonly CredentialIssuerOptions _options;
        private readonly ICredIssuerTokenHelper _tokenHelper;

        public CredIssuerPreAuthorizedCodeHandler(ICredentialOfferRepository credentialOfferRepository, IOptions<CredentialIssuerOptions> opts, ICredIssuerTokenHelper tokenHelper, IPreAuthorizedCodeValidator validator, IBusControl busControl, IClientAuthenticationHelper clientAuthenticationHelper, IGrantedTokenHelper grantedTokenHelper, IEnumerable<ITokenProfile> tokenProfiles, IEnumerable<ITokenBuilder> tokenBuilders, IGrantHelper audienceHelper, IOptions<IdServerHostOptions> options) : base(validator, busControl, clientAuthenticationHelper, tokenProfiles, tokenBuilders, audienceHelper, grantedTokenHelper, options)
        {
            _credentialOfferRepository = credentialOfferRepository;
            _options = opts.Value;
            _tokenHelper = tokenHelper;
        }

        protected override async Task Enrich(HandlerContext handlerContext, JsonObject result, CancellationToken cancellationToken)
        {
            var preAuthorizedCode = handlerContext.Request.RequestData.GetPreAuthorizedCode();
            var credentialOffer = await _credentialOfferRepository.Query().SingleAsync(c => c.PreAuthorizedCode== preAuthorizedCode, cancellationToken);
            credentialOffer.Status = Domains.UserCredentialOfferStatus.ISSUED;
            await _credentialOfferRepository.SaveChanges(cancellationToken);
            var expiresIn = handlerContext.Client.CNonceExpirationTimeInSeconds ?? _options.DefaultCNonceExpirationTimeInSeconds.Value;
            var credentialNonce = Guid.NewGuid().ToString();
            await _tokenHelper.AddCredentialNonce(credentialNonce, expiresIn, cancellationToken);
            result.Add(CredIssuerTokenResponseParameters.CNonce, credentialNonce);
            result.Add(CredIssuerTokenResponseParameters.CNonceExpiresIn, expiresIn);
        }
    }
}
