// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.CredentialIssuer.Api.Credential
{
    public class CredentialController : BaseController
    {
        private readonly IEnumerable<ICredentialRequestParser> _parsers;
        private readonly IEnumerable<IProofValidator> _proofValidators;
        private readonly ICredentialTemplateClaimsExtractor _claimsExtractor;
        private readonly IEnumerable<ICredentialFormat> _formats;
        private readonly ILogger<CredentialController> _logger;

        public CredentialController(
            IEnumerable<ICredentialRequestParser> parsers, 
            IEnumerable<IProofValidator> proofValidators, 
            ICredentialTemplateClaimsExtractor claimsExtractor,
            IEnumerable<ICredentialFormat> formats,
            ILogger<CredentialController> logger)
        {
            _parsers = parsers;
            _proofValidators = proofValidators;
            _claimsExtractor = claimsExtractor;
            _formats = formats;
            _logger = logger;
        }

        [HttpPost(Constants.EndPoints.Credential)]
        [Authorize("Authenticated")]
        public async Task<IActionResult> Get([FromRoute] string prefix, [FromBody] CredentialRequest request, CancellationToken cancellationToken)
        {
            var subject = User.FindFirst("sub").Value;
            if (TryValidate(request, out ErrorResult error)) return Build(error);


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

        private bool Validate(CredentialRequest credentialRequest, out ErrorResult error)
        {
            if (credentialRequest == null)
            {
                error = new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_REQUEST, ErrorMessages.INVALID_INCOMING_REQUEST);
                return false;
            }

            var credentialIdentifier = User.FindFirst("credential_identifier")?.Value;
            if(string.IsNullOrWhiteSpace(credentialIdentifier) && string.IsNullOrWhiteSpace(credentialRequest.Format))
            {
                error = new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, CredentialRequestNames.Format));
                return false;
            }

            if(!string.IsNullOrWhiteSpace(credentialRequest.Format) && !string.IsNullOrWhiteSpace(credentialRequest.Credentialidentifier))
            {
                error = new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_REQUEST, ErrorMessages.CREDENTIAL_IDENTIFIER_MUST_NOT_BE_PRESENT);
                return false;
            }

            if(!string.IsNullOrWhiteSpace(credentialIdentifier) && string.IsNullOrWhiteSpace(credentialRequest.Credentialidentifier))
            {
                error = new ErrorResult(HttpStatusCode.BadRequest, ErrorCodes.INVALID_CREDENTIAL_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, CredentialRequestNames.CredentialIdentifier));
                return false;
            }

            return true;
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

        private List<string> GetCredentialIdentifiers()
        {
            return null;
        }
    }
}
