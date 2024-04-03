// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.CredentialIssuer.Api.CredentialOffer.Commands;
using SimpleIdServer.CredentialIssuer.Api.CredentialOffer.Queries;
using SimpleIdServer.CredentialIssuer.Store;
using SimpleIdServer.IdServer.CredentialIssuer;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.CredentialIssuer.Api.CredentialOffer
{
    [Route(Constants.EndPoints.CredentialOffer)]
    public class CredentialOfferController : BaseController
    {
        private readonly ICreateCredentialOfferCommandHandler _createCredentialOfferCommandHandler;
        private readonly IGetCredentialOfferQueryHandler _getCredentialOfferQueryHandler;
        private readonly ICredentialOfferStore _credentialOfferStore;
        private readonly CredentialIssuerOptions _options;

        public CredentialOfferController(
            ICreateCredentialOfferCommandHandler createCredentialOfferCommandHandler,
            IGetCredentialOfferQueryHandler getCredentialOfferQueryHandler,
            ICredentialOfferStore credentialOfferStore,
            IOptions<CredentialIssuerOptions> options)
        {
            _createCredentialOfferCommandHandler = createCredentialOfferCommandHandler;
            _getCredentialOfferQueryHandler = getCredentialOfferQueryHandler;
            _credentialOfferStore = credentialOfferStore;
            _options = options.Value;
        }

        [HttpPost]
        [Authorize("ApiAuthenticated")]
        public async Task<IActionResult> Create([FromBody] CreateCredentialOfferRequest request, CancellationToken cancellationToken)
        {
            // https://openid.github.io/OpenID4VCI/openid-4-verifiable-credential-issuance-wg-draft.html#name-authorization-code-flow
            // https://curity.io/resources/learn/pre-authorized-code/ - exchange the access token for a pre-authorized code and PIN.
            var accessToken = await GetAccessToken();
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var result = await _createCredentialOfferCommandHandler.Handle(new CreateCredentialOfferCommand
            {
                AccessToken = accessToken,
                CredentialConfigurationIds = request.CredentialConfigurationIds,
                Grants = request.Grants,
                Subject = request.Subject
            }, cancellationToken);
            if (result.Error != null)
                return Build(result.Error.Value);
            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.Created,
                Content = JsonSerializer.Serialize(_getCredentialOfferQueryHandler.ToDto(result.CredentialOffer, issuer, _options.AuthorizationServer)),
                ContentType = "application/json"
            };
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id, CancellationToken cancellationToken)
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var credentialOffer = await _credentialOfferStore.Get(id, cancellationToken);
            if (credentialOffer == null)
                return Build(new ErrorResult(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_OFFER, id)));
            return new OkObjectResult(_getCredentialOfferQueryHandler.Get(new GetCredentialOfferQuery { CredentialOffer = credentialOffer, Issuer = issuer }, _options.AuthorizationServer));
        }

        [HttpGet("{id}/qr")]
        public async Task<IActionResult> GetQrCode(string id, CancellationToken cancellationToken)
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var credentialOffer = await _credentialOfferStore.Get(id, cancellationToken);
            if (credentialOffer == null)
                return Build(new ErrorResult(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_OFFER, id)));
            return File(_getCredentialOfferQueryHandler.GetQrCode(new GetCredentialOfferQuery { CredentialOffer = credentialOffer, Issuer = issuer }, _options.AuthorizationServer), "image/png");
        }
    }
}
