// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using QRCoder;
using SimpleIdServer.CredentialIssuer.Domains;
using SimpleIdServer.CredentialIssuer.Services;
using SimpleIdServer.CredentialIssuer.Store;
using SimpleIdServer.IdServer.CredentialIssuer;
using SimpleIdServer.IdServer.CredentialIssuer.Api.CredentialOffer;
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SimpleIdServer.CredentialIssuer.Api.CredentialOffer
{
    [Route(Constants.EndPoints.CredentialOffer)]
    public class CredentialOfferController : BaseController
    {
        private readonly ICredentialConfigurationStore _credentialConfigurationStore;
        private readonly ICredentialOfferStore _credentialOfferStore;
        private readonly IPreAuthorizedCodeService _preAuthorizedCodeService;
        private readonly CredentialIssuerOptions _options;

        public CredentialOfferController(
            ICredentialConfigurationStore credentialConfigurationStore,
            ICredentialOfferStore credentialOfferStore,
            IPreAuthorizedCodeService preAuthorizedCodeService,
            IOptions<CredentialIssuerOptions> options)
        {
            _credentialConfigurationStore = credentialConfigurationStore;
            _credentialOfferStore = credentialOfferStore;
            _preAuthorizedCodeService = preAuthorizedCodeService;
            _options = options.Value;
        }

        [HttpPost]
        [Authorize("Authenticated")]
        public async Task<IActionResult> Create([FromBody] CreateCredentialOfferRequest request, CancellationToken cancellationToken)
        {
            // https://openid.github.io/OpenID4VCI/openid-4-verifiable-credential-issuance-wg-draft.html#name-authorization-code-flow
            // https://curity.io/resources/learn/pre-authorized-code/ - exchange the access token for a pre-authorized code and PIN.
            var accessToken = Request.Headers
                .Single(h => h.Key == "Authorization")
                .Value
                .Single()
                .Split(" ")
                .Last();
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var validationResult = await Validate(request, cancellationToken);
            if (validationResult.ErrorResult != null)
                return Build(validationResult.ErrorResult.Value);
            var credentialOffer = validationResult.CredentialOffer;
            if(credentialOffer.GrantTypes.Contains(CredentialOfferResultNames.AuthorizedCodeGrant))
            {
                credentialOffer.IssuerState = Guid.NewGuid().ToString();
            }

            if(credentialOffer.GrantTypes.Contains(CredentialOfferResultNames.PreAuthorizedCodeGrant))
            {
                credentialOffer.PreAuthorizedCode = await _preAuthorizedCodeService.Get(accessToken, cancellationToken);
            }

            _credentialOfferStore.Add(credentialOffer);
            await _credentialOfferStore.SaveChanges(cancellationToken);
            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.Created,
                Content = JsonSerializer.Serialize(ToDto(credentialOffer, issuer)),
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
            return new OkObjectResult(ToOfferDtoResult(credentialOffer, issuer));
        }

        [HttpGet("{id}/qr")]
        public async Task<IActionResult> GetQrCode(string id, CancellationToken cancellationToken)
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var credentialOffer = await _credentialOfferStore.Get(id, cancellationToken);
            if (credentialOffer == null)
                return Build(new ErrorResult(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_OFFER, id)));
            var dto = ToDto(credentialOffer, issuer);
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(dto.OfferUri, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            var payload = qrCode.GetGraphic(20);
            return File(payload, "image/png");
        }

        private async Task<CredentialOfferValidationResult> Validate(CreateCredentialOfferRequest request, CancellationToken cancellationToken)
        {
            if (request == null) 
                return CredentialOfferValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_OFFER_REQUEST, ErrorMessages.INVALID_INCOMING_REQUEST));
            if (request.Grants == null || !request.Grants.Any())
                return CredentialOfferValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_OFFER_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, CredentialOfferResultNames.Grants)));
            if (request.CredentialConfigurationIds == null || !request.CredentialConfigurationIds.Any())
                return CredentialOfferValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_OFFER_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, CredentialOfferResultNames.CredentialConfigurationIds)));
            if (string.IsNullOrWhiteSpace(request.Subject))
                return CredentialOfferValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_OFFER_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, "sub")));
            var invalidGrants = request.Grants.Where(g => g != CredentialOfferResultNames.PreAuthorizedCodeGrant && g != CredentialOfferResultNames.AuthorizedCodeGrant);
            if (invalidGrants.Any())
                return CredentialOfferValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_OFFER_REQUEST, string.Format(ErrorMessages.UNSUPPORTED_GRANT_TYPES, string.Join(',', invalidGrants))));
            var existingCredentials = await _credentialConfigurationStore.GetByServerIds(request.CredentialConfigurationIds, cancellationToken);
            var unknownCredentials = request.CredentialConfigurationIds.Where(id => !existingCredentials.Any(c => c.Id == id));
            if (unknownCredentials.Any())
                return CredentialOfferValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_OFFER_REQUEST, string.Format(ErrorMessages.UNSUPPORTED_CREDENTIAL, string.Join(',', unknownCredentials))));
            var credentialOffer = new Domains.CredentialOfferRecord
            {
                Id = Guid.NewGuid().ToString(),
                GrantTypes = request.Grants,
                CredentialConfigurationIds = request.CredentialConfigurationIds,
                Subject = request.Subject,
                CreateDateTime = DateTime.UtcNow,
            };
            return CredentialOfferValidationResult.Ok(credentialOffer);
        }

        private CredentialOfferRecordResult ToDto(CredentialOfferRecord credentialOffer, string issuer)
        {
            var result = new CredentialOfferRecordResult
            {
                Id = credentialOffer.Id,
                GrantTypes = credentialOffer.GrantTypes,
                Subject = credentialOffer.Subject,
                CredentialConfigurationIds = credentialOffer.CredentialConfigurationIds,
                CreateDateTime = credentialOffer.CreateDateTime,
                Offer = ToOfferDtoResult(credentialOffer, issuer)
            };
            if (_options.IsCredentialOfferReturnedByReference)
                result.OfferUri = SerializeByReference(result, issuer);
            else
                result.OfferUri = SerializeByValue(result);

            return result;
        }

        private CredentialOfferResult ToOfferDtoResult(CredentialOfferRecord credentialOffer, string issuer)
        {
            AuthorizedCodeGrant authorizedCodeGrant = null;
            PreAuthorizedCodeGrant preAuthorizedCodeGrant = null;
            if (credentialOffer.GrantTypes.Contains(CredentialOfferResultNames.AuthorizedCodeGrant))
            {
                authorizedCodeGrant = new AuthorizedCodeGrant
                {
                    IssuerState = credentialOffer.IssuerState
                };
            }

            if (credentialOffer.GrantTypes.Contains(CredentialOfferResultNames.PreAuthorizedCodeGrant))
            {
                preAuthorizedCodeGrant = new PreAuthorizedCodeGrant
                {
                    PreAuthorizedCode = credentialOffer.PreAuthorizedCode
                };
            }

            return new CredentialOfferResult
            {
                CredentialIssuer = issuer,
                CredentialConfigurationIds = credentialOffer.CredentialConfigurationIds,
                Grants = new CredentialOfferGrants
                {
                    AuthorizedCodeGrant = authorizedCodeGrant,
                    PreAuthorizedCodeGrant = preAuthorizedCodeGrant
                }
            };
        }

        private string SerializeByReference(CredentialOfferRecordResult offerRecord, string issuer)
        {
            var action = Url.Action("Get", new { id = offerRecord.Id });
            var url = $"{issuer}{action}";
            return $"openid-credential-offer://?credential_offer_uri={url}";
        }

        private string SerializeByValue(CredentialOfferRecordResult offerRecord)
        {
            var json = JsonSerializer.Serialize(offerRecord.Offer);
            var encodedJson = HttpUtility.UrlEncode(json);
            return $"openid-credential-offer://?credential_offer={encodedJson}";
        }

        private class CredentialOfferValidationResult : BaseValidationResult
        {
            private CredentialOfferValidationResult(Domains.CredentialOfferRecord credentialOffer)
            {
                CredentialOffer = credentialOffer;   
            }

            private CredentialOfferValidationResult(ErrorResult error) : base(error)
            {
            }

            public Domains.CredentialOfferRecord CredentialOffer { get; private set; }

            public static CredentialOfferValidationResult Ok(Domains.CredentialOfferRecord credentialOffer) => new CredentialOfferValidationResult(credentialOffer);

            public static CredentialOfferValidationResult Error(ErrorResult error) => new CredentialOfferValidationResult(error);
        }
    }
}
