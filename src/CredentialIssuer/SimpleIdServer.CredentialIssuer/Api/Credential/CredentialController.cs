// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.CredentialIssuer.Api.Credential.Validators;
using SimpleIdServer.CredentialIssuer.CredentialFormats;
using SimpleIdServer.CredentialIssuer.Store;
using SimpleIdServer.IdServer.CredentialIssuer;
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.CredentialIssuer.Api.Credential
{
    [Route(Constants.EndPoints.Credential)]
    [Authorize("ApiAuthenticated")]
    public class CredentialController : BaseController
    {
        private readonly IEnumerable<ICredentialFormatter> _formatters;
        private readonly ICredentialStore _credentialStore;
        private readonly ICredentialConfigurationStore _credentialConfigurationStore;
        private readonly ICredentialOfferStore _credentialOfferStore;
        private readonly IDeferredCredentialStore _deferredCredentialStore;
        private readonly IUserCredentialClaimStore _userCredentialClaimStore;
        private readonly IEnumerable<IKeyProofTypeValidator> _keyProofTypeValidators;
        private readonly CredentialIssuerOptions _options;

        public CredentialController(
            IEnumerable<ICredentialFormatter> formatters,
            ICredentialStore credentialStore,
            ICredentialConfigurationStore credentialConfigurationStore,
            ICredentialOfferStore credentialOfferStore,
            IDeferredCredentialStore deferredCredentialStore,
            IUserCredentialClaimStore userCredentialClaimStore,
            IEnumerable<IKeyProofTypeValidator> keyProofTypeValidators,
            IOptions<CredentialIssuerOptions> options)
        {
            _formatters = formatters;
            _credentialStore = credentialStore;
            _credentialConfigurationStore = credentialConfigurationStore;
            _credentialOfferStore = credentialOfferStore;
            _deferredCredentialStore = deferredCredentialStore;
            _userCredentialClaimStore = userCredentialClaimStore;
            _keyProofTypeValidators = keyProofTypeValidators;
            _options = options.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Get([FromBody] CredentialRequest request, CancellationToken cancellationToken)
        {
            var scope = User.Claims.SingleOrDefault(c => c.Type == "scope")?.Value;
            var authorizedScopes = new List<string>();
            if (!string.IsNullOrWhiteSpace(scope))
                authorizedScopes = scope.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();

            var validationResult = await Validate(request, authorizedScopes, cancellationToken);
            if (validationResult.ErrorResult != null) return Build(validationResult.ErrorResult.Value);
            if (validationResult.CredentialConfiguration.IsDeferred)
                return new OkObjectResult(await BuildDeferredCredential(validationResult, cancellationToken));

            return new OkObjectResult(await BuildImmediateCredential(request, validationResult, cancellationToken));
        }

        #region Deferred credential

        private async Task<CredentialResult> BuildDeferredCredential(
            CredentialValidationResult validationResult,
            CancellationToken cancellationToken)
        {
            var deferredCredential = new Domains.DeferredCredential
            {
                Status = Domains.DeferredCredentialStatus.PENDING,
                TransactionId = Guid.NewGuid().ToString(),
                CredentialConfigurationId = validationResult.CredentialConfiguration.Id,
                CredentialId = validationResult.Credential?.Id
            };
            _deferredCredentialStore.Add(deferredCredential);
            await _deferredCredentialStore.SaveChanges(cancellationToken);
            return new CredentialResult
            {
                TransactionId = deferredCredential.TransactionId,   
                CNonce = validationResult.Nonce
            };

        }

        #endregion

        #region Immediate credential

        private async Task<CredentialResult> BuildImmediateCredential(CredentialRequest request,
            CredentialValidationResult validationResult, 
            CancellationToken cancellationToken)
        {
            var userDid = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var buildRequest = new BuildCredentialRequest
            {
                Subject = validationResult.Subject,
                Issuer = _options.DidDocument.Id,
                JsonLdContext = validationResult.CredentialConfiguration.JsonLdContext,
                Type = validationResult.CredentialConfiguration.Type,
                CredentialConfiguration = validationResult.CredentialConfiguration,
                AdditionalTypes = validationResult.CredentialConfiguration.AdditionalTypes,
            };

            if (!string.IsNullOrWhiteSpace(validationResult.CredentialConfiguration.CredentialSchemaId))
            {
                buildRequest.Schema = new CredentialSchema
                {
                    Id = validationResult.CredentialConfiguration.CredentialSchemaId,
                    Type = validationResult.CredentialConfiguration.CredentialSchemaType
                };
            }

            Dictionary<string, string> claims = null;
            if (validationResult.Credential != null)
                claims = Enrich(buildRequest, validationResult.Credential);
            else
                claims = await Enrich(buildRequest, validationResult.CredentialConfiguration, cancellationToken);

            buildRequest.UserClaims = claims.Select(kvp =>
            {
                var cl = validationResult.CredentialConfiguration.Claims.Single(cl => cl.SourceUserClaimName == kvp.Key);
                return new CredentialUserClaimNode
                {
                    Level = cl.Name.Split('.').Count(),
                    Name = cl.Name,
                    Value = kvp.Value
                };
            }).ToList();
            var formatter = validationResult.Formatter;
            var credentialResult = formatter.Build(buildRequest,
                _options.DidDocument,
                _options.VerificationMethodId,
                _options.AsymmKey);
            if (request.CredentialResponseEncryption != null)
            {
                var handler = new JsonWebTokenHandler();
                var encKey = request.CredentialResponseEncryption.Jwk;
                var encryptedCredential = handler.EncryptToken(credentialResult.ToJsonString(), new Microsoft.IdentityModel.Tokens.EncryptingCredentials(
                encKey,
                    request.CredentialResponseEncryption.Alg,
                    request.CredentialResponseEncryption.Enc));
                credentialResult = encryptedCredential;
            }

            return new CredentialResult
            {
                Format = validationResult.Formatter.Format,
                Credential = credentialResult,
                CNonce = validationResult.Nonce
            };
        }

        private Dictionary<string, string> Enrich(
            BuildCredentialRequest buildRequest, 
            Domains.Credential credential)
        {
            buildRequest.Id = $"{credential.Configuration.BaseUrl}/{credential.CredentialId}";
            buildRequest.ValidFrom = credential.IssueDateTime;
            buildRequest.ValidUntil = credential.ExpirationDateTime;
            return credential.Claims.ToDictionary(c => c.Name, c => c.Value);
        }

        private async Task<Dictionary<string, string>> Enrich(
            BuildCredentialRequest buildRequest,
            Domains.CredentialConfiguration credentialConfiguration,
            CancellationToken cancellationToken)
        {
            buildRequest.Id = $"{credentialConfiguration.BaseUrl}/{Guid.NewGuid()}";
            buildRequest.ValidFrom = DateTime.UtcNow.Date;
            if (_options.CredentialExpirationTimeInSeconds != null)
            {
                buildRequest.ValidUntil = DateTime.UtcNow.Date.AddSeconds(_options.CredentialExpirationTimeInSeconds.Value);
            }

            if (!string.IsNullOrWhiteSpace(credentialConfiguration.CredentialSchemaId))
            {
                buildRequest.Schema = new CredentialSchema
                {
                    Id = credentialConfiguration.CredentialSchemaId,
                    Type = credentialConfiguration.CredentialSchemaType
                };
            }

            var userCredentials = await _userCredentialClaimStore.Resolve(buildRequest.Subject, credentialConfiguration.Claims, cancellationToken);
            return userCredentials.ToDictionary(c => c.Name, c => c.Value);
        }

        #endregion

        private async Task<CredentialValidationResult> Validate(
            CredentialRequest credentialRequest, 
            List<string> authorizedScopes, 
            CancellationToken cancellationToken)
        {
            string subject = null;
            string nonce = null;
            if (credentialRequest == null)
                return CredentialValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_REQUEST, ErrorMessages.INVALID_INCOMING_REQUEST));

            var atCredentialIdentifiers = GetCredentialIdentifiers();
            if(atCredentialIdentifiers == null && string.IsNullOrWhiteSpace(credentialRequest.Format))
                return CredentialValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, CredentialRequestNames.Format)));

            if(credentialRequest.Proof != null)
            {
                if (string.IsNullOrWhiteSpace(credentialRequest.Proof.ProofType)) return CredentialValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, CredentialRequestNames.ProofType)));
                var proofType = _keyProofTypeValidators.SingleOrDefault(v => v.Type == credentialRequest.Proof.ProofType);
                if (proofType == null) return CredentialValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_REQUEST, string.Format(ErrorMessages.INVALID_PROOF_FORMAT, credentialRequest.Proof.ProofType)));
                var proofTypeValidationResult = await proofType.Validate(credentialRequest.Proof, cancellationToken);
                if (!proofTypeValidationResult.IsValid) return CredentialValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_PROOF, proofTypeValidationResult.ErrorMessage));
                subject = proofTypeValidationResult.Subject;
                nonce = proofTypeValidationResult.CNonce;
            }

            if (atCredentialIdentifiers != null && string.IsNullOrWhiteSpace(credentialRequest.Credentialidentifier))
                return CredentialValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, CredentialRequestNames.CredentialIdentifier)));

            if(!string.IsNullOrWhiteSpace(credentialRequest.Format) && !string.IsNullOrWhiteSpace(credentialRequest.Credentialidentifier))
                return CredentialValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_REQUEST, ErrorMessages.CANNOT_USER_CREDENTIAL_IDENTIFIER_WITH_FORMAT));

            if(!string.IsNullOrWhiteSpace(credentialRequest.Credentialidentifier) && !atCredentialIdentifiers.Contains(credentialRequest.Credentialidentifier))
                return CredentialValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_REQUEST, ErrorMessages.INVALID_CREDENTIAL_IDENTIFIER));

            if (credentialRequest.CredentialResponseEncryption != null)
            {
                if (string.IsNullOrWhiteSpace(credentialRequest.CredentialResponseEncryption.Alg))
                    return CredentialValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_ENCRYPTION_PARAMETERS, string.Format(ErrorMessages.MISSING_PARAMETER, CredentialRequestNames.Alg)));
                if (string.IsNullOrWhiteSpace(credentialRequest.CredentialResponseEncryption.Enc))
                    return CredentialValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_ENCRYPTION_PARAMETERS, string.Format(ErrorMessages.MISSING_PARAMETER, CredentialRequestNames.Enc)));
                if (credentialRequest.CredentialResponseEncryption.Jwk == null)
                    return CredentialValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_ENCRYPTION_PARAMETERS, string.Format(ErrorMessages.MISSING_PARAMETER, CredentialRequestNames.Jwk)));
            }

            if (!string.IsNullOrWhiteSpace(credentialRequest.Format))
            {
                var formatter = _formatters.SingleOrDefault(f => f.Format == credentialRequest.Format);
                if (formatter == null) return CredentialValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.UNSUPPORTED_CREDENTIAL_FORMAT, string.Format(ErrorMessages.UNSUPPORTED_CREDENTIAL_FORMAT, credentialRequest.Format)));
                var header = formatter.ExtractHeader(credentialRequest.Data);
                if (header == null) return CredentialValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_REQUEST, ErrorMessages.CREDENTIAL_TYPE_CANNOT_BE_EXTRACTED));
                var credentialConfiguration = await _credentialConfigurationStore.GetByTypeAndFormat(header.Type, credentialRequest.Format, cancellationToken);
                if (credentialConfiguration == null) return CredentialValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.UNSUPPORTED_CREDENTIAL_TYPE, string.Format(ErrorMessages.UNSUPPORTED_CREDENTIAL_TYPE, header.Type)));
                if (!string.IsNullOrWhiteSpace(credentialConfiguration.Scope) && !authorizedScopes.Any(s => credentialConfiguration.Scope == s)) return CredentialValidationResult.Error(new ErrorResult(HttpStatusCode.Unauthorized, ErrorCodes.UNAUTHORIZED, string.Format(ErrorMessages.UNAUTHORIZED_TO_ACCESS, header.Type))); 
                return CredentialValidationResult.Ok(formatter, credentialConfiguration, subject, nonce);
            }

            var credential = await _credentialStore.GetByCredentialId(credentialRequest.Credentialidentifier, cancellationToken);
            if (credential == null) return CredentialValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_REQUEST, string.Format(ErrorMessages.UNKNOWN_CREDENTIAL_ID, credentialRequest.Credentialidentifier)));
            return CredentialValidationResult.Ok(_formatters.Single(f => f.Format == credential.Configuration.Format), credential, subject, nonce);
        }

        private class CredentialValidationResult : BaseValidationResult
        {
            private CredentialValidationResult(ErrorResult error) : base(error)
            {
            }

            private CredentialValidationResult(ICredentialFormatter formatter, Domains.CredentialConfiguration credentialConfiguration, string subject, string nonce)
            {
                Formatter = formatter;
                CredentialConfiguration = credentialConfiguration;
                Subject = subject;
                Nonce = nonce;
            }

            private CredentialValidationResult(ICredentialFormatter formatter, Domains.Credential credential, string subject, string nonce) : this(formatter,  credential.Configuration, subject, nonce)
            {
                Credential = credential;
            }

            public ICredentialFormatter Formatter { get; private set; }

            public Domains.CredentialConfiguration CredentialConfiguration { get; private set; }

            public string Subject { get; private set; }

            public string Nonce { get; private set; }

            public Domains.Credential Credential { get; private set; }

            public static CredentialValidationResult Ok(ICredentialFormatter formatter, Domains.CredentialConfiguration credentialConfiguration, string subject, string nonce) 
                => new CredentialValidationResult(formatter, credentialConfiguration, subject, nonce);

            public static CredentialValidationResult Ok(ICredentialFormatter formatter, Domains.Credential credential, string subject, string nonce) => new CredentialValidationResult(formatter, credential, subject, nonce);

            public static CredentialValidationResult Error(ErrorResult error) => new CredentialValidationResult(error);
        }

        private List<string> GetCredentialIdentifiers()
        {
            var claim = User.Claims.SingleOrDefault(c => c.Type == "authorization_details");
            if (claim == null) return null;
            var jsonObj = JsonObject.Parse(claim.Value).AsObject();
            if (!jsonObj.ContainsKey("credential_identifiers")) return null;
            return (jsonObj["credential_identifiers"] as JsonArray).Select(c => c.ToString()).ToList();
        }
    }
}
