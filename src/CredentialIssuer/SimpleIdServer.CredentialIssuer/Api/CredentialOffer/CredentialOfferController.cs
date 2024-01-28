// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.CredentialIssuer.Domains;
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
    public class CredentialOfferController : BaseController
    {
        private readonly ICredentialTemplateStore _credentialTemplateStore;
        private readonly ICredentialOfferStore _credentialOfferStore;
        private readonly CredentialIssuerOptions _options;

        public CredentialOfferController(
            ICredentialTemplateStore credentialTemplateStore,
            ICredentialOfferStore credentialOfferStore,
            IOptions<CredentialIssuerOptions> options)
        {
            _credentialTemplateStore = credentialTemplateStore;
            _credentialOfferStore = credentialOfferStore;
            _options = options.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCredentialOfferRequest request, CancellationToken cancellationToken)
        {
            // https://openid.github.io/OpenID4VCI/openid-4-verifiable-credential-issuance-wg-draft.html#name-authorization-code-flow
            // https://curity.io/resources/learn/pre-authorized-code/ - exchange the access token for a pre-authorized code and PIN.
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var validationResult = await Validate(request, cancellationToken);
            if (validationResult.ErrorResult != null)
                return Build(validationResult.ErrorResult.Value);
            var credentialOffer = validationResult.CredentialOffer;
            if(credentialOffer.GrantTypes.Contains(CredentialOfferResultNames.AuthorizedCodeGrant))
            {
                credentialOffer.IssuerState = Guid.NewGuid().ToString();
            }
            else
            {
                // call the token endpoint to get a pre-authorized-code.
                credentialOffer.PreAuthorizedCode = Guid.NewGuid().ToString();
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
            // Generate the QR Code.
            return null;
        }

        /*
        private async Task<CredentialOfferBuildResult> InternalGet(string prefix, string id, CancellationToken cancellationToken)
        {
            prefix = prefix ?? SimpleIdServer.IdServer.Constants.DefaultRealm;
            var credentialOffer = await _credentialOfferRepository.Query().AsNoTracking().SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
            if (credentialOffer == null) return CredentialOfferBuildResult.NotFound(ErrorCodes.INVALID_REQUEST,  ErrorMessages.UNKNOWN_CREDENTIAL_OFFER);
            if (credentialOffer.Status == UserCredentialOfferStatus.INVALID) return CredentialOfferBuildResult.Invalid(ErrorCodes.INVALID_CREDOFFER, ErrorMessages.CREDOFFER_IS_INVALID);
            if (credentialOffer.ExpirationDateTime <= DateTime.UtcNow) return CredentialOfferBuildResult.Invalid(ErrorCodes.INVALID_CREDOFFER, ErrorMessages.CREDOFFER_IS_EXPIRED);
            var client = await _clientRepository.Query().Include(c => c.Realms).FirstAsync(c => c.ClientId == credentialOffer.ClientId && c.Realms.Any(r => r.Name == prefix), cancellationToken);
            var issuer = HandlerContext.GetIssuer(Request.GetAbsoluteUriWithVirtualPath(), _options.UseRealm);
            return InternalGet(prefix, credentialOffer, client);
        }

        private CredentialOfferBuildResult InternalGet(string prefix, UserCredentialOffer credentialOffer, Client client)
        {
            prefix = prefix ?? SimpleIdServer.IdServer.Constants.DefaultRealm;
            var issuer = HandlerContext.GetIssuer(Request.GetAbsoluteUriWithVirtualPath(), _options.UseRealm);
            var result = new CredentialOfferResult
            {
                CredentialIssuer = issuer,
                Credentials = credentialOffer.CredentialNames,
                Grants = new Dictionary<string, object>()
            };
            if (client.GrantTypes.Contains(PreAuthorizedCodeHandler.GRANT_TYPE))
                result.Grants.Add(PreAuthorizedCodeHandler.GRANT_TYPE, new Dictionary<string, object>
                {
                    { CredentialOfferResultNames.UserPinRequired, client.UserPinRequired },
                    { CredentialOfferResultNames.PreAuthorizedCode, credentialOffer.PreAuthorizedCode }
                });

            if (client.GrantTypes.Contains(AuthorizationCodeHandler.GRANT_TYPE))
            {
                result.Grants.Add(AuthorizationCodeHandler.GRANT_TYPE, new Dictionary<string, object>
                {
                    { CredentialOfferResultNames.IssuerState, credentialOffer.CredIssuerState }
                });
            }

            var url = BuildUrl(result, client);
            return CredentialOfferBuildResult.Ok(url, credentialOffer);
        }

        private async Task<(IActionResult, CredentialOfferBuildResult)> InternalShare(string prefix, string nameIdentifier, ShareCredentialTemplateRequest request, CancellationToken cancellationToken)
        {
            prefix = prefix ?? IdServer.Constants.DefaultRealm;
            if (request == null) return (BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.MALFROMED_INCOMING_REQUEST), null);
            if (string.IsNullOrWhiteSpace(request.WalletClientId)) return (BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, ShareCredentialTemplateNames.WalletClientId)), null);
            if (string.IsNullOrWhiteSpace(request.CredentialTemplateId)) return (BuildError(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, ShareCredentialTemplateNames.CredentialTemplateId)), null);
            var credentialTemplate = await _credentialTemplateRepository.Query().AsNoTracking().SingleOrDefaultAsync(c => c.TechnicalId == request.CredentialTemplateId, cancellationToken);
            if (credentialTemplate == null) return (BuildError(HttpStatusCode.NotFound, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_TEMPLATE, request.CredentialTemplateId)), null);
            var client = await _clientRepository.Query().AsNoTracking().SingleOrDefaultAsync(c => c.ClientId == request.WalletClientId && c.ClientType == ClientTypes.WALLET, cancellationToken);
            if (client == null) return (BuildError(HttpStatusCode.NotFound, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_WALLET_CLIENT_ID, request.WalletClientId)), null);
            var user = await _authenticationHelper.GetUserByLogin(nameIdentifier, prefix, cancellationToken);
            if (user == null) return (BuildError(HttpStatusCode.Unauthorized, ErrorCodes.UNAUTHORIZED, string.Format(ErrorMessages.UNKNOWN_USER, nameIdentifier)), null);
            var cred = await _credentialOfferRepository.Query().AsNoTracking().FirstOrDefaultAsync(c => c.CredentialTemplateId == request.CredentialTemplateId && c.UserId == user.Id && c.ClientId == request.WalletClientId);

            if (cred == null || cred.Status != UserCredentialOfferStatus.VALID || cred.ExpirationDateTime <= DateTime.UtcNow)
            {
                cred = new UserCredentialOffer
                {
                    Id = Guid.NewGuid().ToString(),
                    UpdateDateTime = DateTime.UtcNow,
                    CreateDateTime = DateTime.UtcNow,
                    ExpirationDateTime = DateTime.UtcNow.AddSeconds(_options.CredOfferExpirationInSeconds),
                    Status = UserCredentialOfferStatus.VALID,
                    ClientId = request.WalletClientId,
                    UserId = user.Id,
                    CredentialTemplateId = request.CredentialTemplateId
                };
                _credentialOfferRepository.Add(cred);
                if (client.GrantTypes.Contains(PreAuthorizedCodeHandler.GRANT_TYPE))
                {
                    string pin = string.Empty;
                    if (client.UserPinRequired)
                    {
                        pin = Guid.NewGuid().ToString();
                        var notificationService = _notificationServices.First(n => (user.NotificationMode ?? IdServer.Constants.DefaultNotificationMode) == n.Name);
                        await notificationService.Send("PIN", $"The pin is {pin}", new Dictionary<string, string>(), user);
                    }

                    var preAuthorizedCode = Guid.NewGuid().ToString();
                    cred.PreAuthorizedCode = preAuthorizedCode;
                    cred.Pin = pin;
                    await _grantedTokenHelper.AddPreAuthCode(cred.PreAuthorizedCode, cred.Pin, cred.ClientId, cred.UserId, _options.CredOfferExpirationInSeconds, cancellationToken);
                }

                if (client.GrantTypes.Contains(AuthorizationCodeHandler.GRANT_TYPE))
                {
                    var credIssuerState = Guid.NewGuid().ToString();
                    cred.CredIssuerState = credIssuerState;
                }

                await _credentialOfferRepository.SaveChanges(cancellationToken);
            }

            var result = InternalGet(prefix, cred, client);
            return (null, result);
        }

        private string BuildUrl(CredentialOfferResult result, Client client)
        {
            var res = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(CredentialOfferResultNames.CredentialOffer, _urlEncoder.Encode(JsonSerializer.Serialize(result)))
            };
            var queryCollection = new QueryBuilder(res);
            if (!string.IsNullOrWhiteSpace(client.CredentialOfferEndpoint))
                return $"{client.CredentialOfferEndpoint}{queryCollection.ToQueryString()}";

            return $"openid-credential-offer://{queryCollection.ToQueryString()}";
        }

        private FileContentResult GetQRCode(CredentialOfferBuildResult res)
        {
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(res.Url, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            var payload = qrCode.GetGraphic(20);
            return File(payload, "image/png");
        }

        private class CredentialOfferBuildResult
        {
            private CredentialOfferBuildResult() { }

            public string Url { get; private set; }
            public UserCredentialOffer CredentialOffer { get; private set; }
            public HttpStatusCode Code { get; private set; }
            public string ErrorCode { get; private set; }
            public string ErrorMessage { get; private set; }

            public static CredentialOfferBuildResult Ok(string url, UserCredentialOffer credentialOffer)
            {
                return new CredentialOfferBuildResult
                {
                    Code = HttpStatusCode.OK,
                    Url = url,
                    CredentialOffer = credentialOffer
                };
            }

            public static CredentialOfferBuildResult Unauthorized(string errorCode, string errorMessage)
            {
                return new CredentialOfferBuildResult
                {
                    Code = HttpStatusCode.Unauthorized,
                    ErrorCode = errorCode,
                    ErrorMessage = errorMessage
                };
            }

            public static CredentialOfferBuildResult NotFound(string errorCode, string errorMessage)
            {
                return new CredentialOfferBuildResult
                {
                    Code = HttpStatusCode.NotFound,
                    ErrorCode = errorCode,
                    ErrorMessage = errorMessage
                };
            }

            public static CredentialOfferBuildResult Invalid(string errorCode, string errorMessage) 
            {
                return new CredentialOfferBuildResult
                {
                    Code = HttpStatusCode.InternalServerError,
                    ErrorCode= errorCode,
                    ErrorMessage = errorMessage
                };
            }
        }
        */

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
            var existingCredentials = await _credentialTemplateStore.Get(request.CredentialConfigurationIds, cancellationToken);
            var unknownCredentials = request.CredentialConfigurationIds.Where(id => !existingCredentials.Any(c => c.Id != id));
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
                    IssuerState = issuer
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
