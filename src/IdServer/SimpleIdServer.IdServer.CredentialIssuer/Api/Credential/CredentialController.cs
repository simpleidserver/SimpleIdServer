// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.CredentialIssuer.Api.Credential.Validators;
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using SimpleIdServer.IdServer.CredentialIssuer.Parsers;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Store;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.CredentialIssuer.Api.Credential
{
    public class CredentialController : BaseController
    {
        private readonly IEnumerable<ICredentialRequestParser> _parsers;
        private readonly IEnumerable<IProofValidator> _proofValidators;
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IUserRepository _userRepository;
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly ILogger<CredentialController> _logger;

        public CredentialController(IEnumerable<ICredentialRequestParser> parsers, IEnumerable<IProofValidator> proofValidators, IGrantedTokenHelper grantedTokenHelper, IUserRepository userRepository, IAuthenticationHelper authenticationHelper, ILogger<CredentialController> logger)
        {
            _parsers = parsers;
            _proofValidators = proofValidators;
            _grantedTokenHelper = grantedTokenHelper;
            _userRepository = userRepository;
            _authenticationHelper = authenticationHelper;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Get([FromRoute] string prefix, [FromBody] CredentialRequest request, CancellationToken cancellationToken)
        {
            prefix = prefix ?? SimpleIdServer.IdServer.Constants.DefaultRealm;
            try
            {
                var accessToken = ExtractBearerToken();
                var token = await _grantedTokenHelper.GetAccessToken(accessToken, cancellationToken);
                if (token == null) return BuildError(HttpStatusCode.Unauthorized, ErrorCodes.INVALID_TOKEN, ErrorMessages.UNKNOWN_ACCESS_TOKEN);
                var extractionResult = await ValidateRequest(request, token, cancellationToken);
                var user = await _authenticationHelper.GetUserByLogin(_userRepository.Query().AsNoTracking(), token.Subject, prefix, cancellationToken);
                await CheckProof(request, user,cancellationToken);
                return null;
            }
            catch (OAuthUnauthorizedException ex)
            {
                _logger.LogError(ex.ToString());
                return BuildError(HttpStatusCode.Unauthorized, ex.Code, ex.Message);
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                return BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }

        protected async Task<ICredentialRequest> ValidateRequest(CredentialRequest request, JsonWebToken jsonWebToken, CancellationToken cancellationToken)
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
                return extractionResult.Request;
            }

            throw new OAuthUnauthorizedException(ErrorCodes.INVALID_REQUEST, ErrorMessages.UNAUHTORIZED_TO_ACCESS_TO_CREDENTIAL);
        }

        protected async Task CheckProof(CredentialRequest request, User user, CancellationToken cancellationToken)
        {
            if (request.Proof == null) return;
            if (string.IsNullOrWhiteSpace(request.Proof.ProofType)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, CredentialRequestNames.ProofType));
            var proofValidator = _proofValidators.FirstOrDefault(v => v.Type == request.Proof.ProofType);
            if (proofValidator == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_PROOF_TYPE, request.Proof.ProofType));
            var validationResult = await proofValidator.Validate(request.Proof, user, cancellationToken);
            if (!string.IsNullOrWhiteSpace(validationResult.ErrorMessage)) throw new OAuthException(ErrorCodes.INVALID_PROOF, validationResult.ErrorMessage);
        }
    }
}
