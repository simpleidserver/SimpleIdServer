// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleIdServer.CredentialIssuer.CredentialFormats;
using SimpleIdServer.CredentialIssuer.Domains;
using SimpleIdServer.CredentialIssuer.Store;
using SimpleIdServer.IdServer.CredentialIssuer;
using SimpleIdServer.IdServer.CredentialIssuer.Api.Credential.Formats;
using SimpleIdServer.IdServer.CredentialIssuer.Api.Credential.Validators;
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using SimpleIdServer.IdServer.CredentialIssuer.Extractors;
using SimpleIdServer.IdServer.CredentialIssuer.Parsers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.CredentialIssuer.Api.Credential
{
    public class CredentialController : BaseController
    {
        private readonly IEnumerable<ICredentialRequestParser> _parsers;
        private readonly IEnumerable<IProofValidator> _proofValidators;
        private readonly ICredentialTemplateClaimsExtractor _claimsExtractor;
        private readonly IEnumerable<ICredentialFormatter> _formatters;
        private readonly ICredentialTemplateStore _credentialTemplateStore;
        private readonly IUserCredentialStore _userCredentialClaimStore;
        private readonly ILogger<CredentialController> _logger;

        public CredentialController(
            IEnumerable<ICredentialRequestParser> parsers, 
            IEnumerable<IProofValidator> proofValidators, 
            ICredentialTemplateClaimsExtractor claimsExtractor,
            IEnumerable<ICredentialFormatter> formatters,
            ICredentialTemplateStore credentialTemplateStore,
            IUserCredentialClaimStore userCredentialClaimStore,
            ILogger<CredentialController> logger)
        {
            _parsers = parsers;
            _proofValidators = proofValidators;
            _claimsExtractor = claimsExtractor;
            _formatters = formatters;
            _credentialTemplateStore = credentialTemplateStore;
            _userCredentialClaimStore = userCredentialClaimStore;
            _logger = logger;
        }

        [HttpPost(Constants.EndPoints.Credential)]
        [Authorize("Authenticated")]
        public async Task<IActionResult> Get([FromRoute] string prefix, [FromBody] CredentialRequest request, CancellationToken cancellationToken)
        {
            var subject = User.FindFirst("sub").Value;
            var validationResult = await Validate(request, cancellationToken);
            if (validationResult.ErrorResult != null) return Build(validationResult.ErrorResult.Value);

            var credentialTemplateClaims = validationResult.CredentialTemplate.Claims;
            var userCredentials = await _userCredentialClaimStore.Resolve(subject, credentialTemplateClaims);
            var formatter = validationResult.Formatter;

            
            // jwt_vs_json, type ["VerifiableCredential", "UniversityDegreeCredential"]
            // mso_mdoc, doctype : org.iso.18013.5.1.mDL
            // CredentialTemplate

            var extractionResult = await ValidateRequest(request, token, cancellationToken);
            var user = await _authenticationHelper.GetUserByLogin(token.Subject, prefix, cancellationToken);
            var validationResult = await CheckProof(request, user, cancellationToken);
            context.SetClient(null);
            context.SetUser(user, null);
            var extractedClaims = await _claimsExtractor.ExtractClaims(context, extractionResult.CredentialTemplate);
            var format = _formats.Single(v => v.Format == request.Format);
            var result = format.Transform(new CredentialFormatParameter(extractedClaims, user, validationResult.IdentityDocument, extractionResult.CredentialTemplate, context.GetIssuer(), validationResult.CNonce, validationResult.CNonceExpiresIn));
            activity?.SetStatus(ActivityStatusCode.Ok, "Credential issued");
            return new OkObjectResult(result);
        }

        private async Task<ValidationResult> Validate(CredentialRequest credentialRequest, CancellationToken cancellationToken)
        {
            if (credentialRequest == null)
            {
                return ValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_REQUEST, ErrorMessages.INVALID_INCOMING_REQUEST));
            }

            var atCredentialIdentifiers = GetCredentialIdentifiers();
            if(atCredentialIdentifiers == null && string.IsNullOrWhiteSpace(credentialRequest.Format))
                return ValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, CredentialRequestNames.Format)));

            // Proof.

            if(atCredentialIdentifiers != null && string.IsNullOrWhiteSpace(credentialRequest.Credentialidentifier))
                return ValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, CredentialRequestNames.CredentialIdentifier)));

            if(!string.IsNullOrWhiteSpace(credentialRequest.Format) && !string.IsNullOrWhiteSpace(credentialRequest.Credentialidentifier))
                return ValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_REQUEST, ErrorMessages.CANNOT_USER_CREDENTIAL_IDENTIFIER_WITH_FORMAT));

            if(!atCredentialIdentifiers.Contains(credentialRequest.Credentialidentifier))
                return ValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_REQUEST, ErrorMessages.INVALID_CREDENTIAL_IDENTIFIER));

            ICredentialFormatter formatter = null;
            CredentialTemplate credentialTemplate = null;
            if (!string.IsNullOrWhiteSpace(credentialRequest.Format))
            {
                formatter = _formatters.SingleOrDefault(f => f.Format == credentialRequest.Format);
                if (formatter == null) return ValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.UNSUPPORTED_CREDENTIAL_FORMAT, string.Format(ErrorMessages.UNSUPPORTED_CREDENTIAL_FORMAT, credentialRequest.Format)));
                var header = formatter.ExtractHeader(credentialRequest.Data);
                if (header == null) return ValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_REQUEST, ErrorMessages.CREDENTIAL_TYPE_CANNOT_BE_EXTRACTED));
                credentialTemplate = await _credentialTemplateStore.Get(header.Type, cancellationToken);
                if (credentialTemplate == null) return ValidationResult.Error(new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.UNSUPPORTED_CREDENTIAL_TYPE, string.Format(ErrorMessages.UNSUPPORTED_CREDENTIAL_TYPE, header.Type)));
            }

            return ValidationResult.Ok(formatter, credentialTemplate);
        }

        protected async Task<ExtractionResult> ValidateRequest(CredentialRequest request, JsonWebToken jsonWebToken, CancellationToken cancellationToken)
        {
            if (request == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.CREDENTIAL_REQUEST_INVALID);
            if (string.IsNullOrWhiteSpace(request.Format)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, CredentialRequestNames.Format));
            var parser = _parsers.SingleOrDefault(p => p.Format == request.Format);
            if (parser == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNSUPPORTED_FORMAT, request.Format));
            var extractionResult = await parser.Extract(request, cancellationToken);
            if (!string.IsNullOrWhiteSpace(extractionResult.ErrorMessage)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, extractionResult.ErrorMessage);
            var serializedClaims = jsonWebToken.GetClaimJson();
            var authDetails = serializedClaims.GetAuthorizationDetailsFromAuthorizationRequest();
            IEnumerable<AuthorizationData> filteredAuthDefailts;
            if (authDetails != null && (filteredAuthDefailts = authDetails.Where(a => a.Type == Constants.StandardAuthorizationDetails.OpenIdCredential)).Any())
            {
                var validationResult = extractionResult.Request.Validate(filteredAuthDefailts);
                if (!string.IsNullOrWhiteSpace(validationResult.ErrorMessage)) throw new OAuthUnauthorizedException(ErrorCodes.INVALID_REQUEST, validationResult.ErrorMessage);
                return extractionResult;
            }

            throw new OAuthUnauthorizedException(ErrorCodes.INVALID_REQUEST, ErrorMessages.UNAUHTORIZED_TO_ACCESS_TO_CREDENTIAL);
        }

        protected async Task<ProofValidationResult> CheckProof(CredentialRequest request, User user, CancellationToken cancellationToken)
        {
            if (request.Proof == null) return null;
            if (string.IsNullOrWhiteSpace(request.Proof.ProofType)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, CredentialRequestNames.ProofType));
            var proofValidator = _proofValidators.FirstOrDefault(v => v.Type == request.Proof.ProofType);
            if (proofValidator == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_PROOF_TYPE, request.Proof.ProofType));
            var validationResult = await proofValidator.Validate(request.Proof, user, cancellationToken);
            if (!string.IsNullOrWhiteSpace(validationResult.ErrorMessage)) throw new OAuthException(ErrorCodes.INVALID_PROOF, validationResult.ErrorMessage);
            return validationResult;
        }

        private record ValidationResult
        {
            public ValidationResult(ErrorResult? error)
            {
                ErrorResult = error;
            }

            public ValidationResult(ICredentialFormatter formatter, CredentialTemplate credentialTemplate)
            {
                Formatter = formatter;
            }

            public ErrorResult? ErrorResult { get; private set; }

            public ICredentialFormatter Formatter { get; private set; }

            public CredentialTemplate CredentialTemplate { get; private set; }

            public static ValidationResult Error(ErrorResult error) => new ValidationResult(error);

            public static ValidationResult Ok(ICredentialFormatter formatter, CredentialTemplate credentialTemplate) => new ValidationResult(formatter, credentialTemplate);
        }

        private List<string> GetCredentialIdentifiers()
        {
            var claim = User.Claims.SingleOrDefault(c => c.Type == "authorization_details");
            if (claim == null) return null;
            var jsonObj = JsonObject.Parse(claim.Value).AsObject();
            if (jsonObj.ContainsKey("credential_identifiers")) return null;
            return (jsonObj["credential_identifiers"] as JsonArray).Select(c => c.ToString()).ToList();
        }
    }
}
