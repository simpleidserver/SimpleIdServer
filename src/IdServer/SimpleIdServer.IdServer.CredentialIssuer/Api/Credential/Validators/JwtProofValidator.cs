﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.Did;
using SimpleIdServer.Did.Jwt;
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.CredentialIssuer.Api.Credential.Validators
{
    public class JwtProofValidator : IProofValidator
    {
        private readonly IEnumerable<IIdentityDocumentExtractor> _didExtractors;

        public JwtProofValidator(IEnumerable<IIdentityDocumentExtractor> didExtractors)
        {
            _didExtractors = didExtractors;
        }

        public string Type => Constants.StandardProofTypes.Jwt;

        public async Task<ProofValidationResult> Validate(CredentialProofRequest request, User user, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(user.Did)) return ProofValidationResult.Error(ErrorMessages.USER_HAS_NO_DID);
            if (!request.Parameters.ContainsKey(CredentialRequestNames.Jwt)) return ProofValidationResult.Error(string.Format(ErrorMessages.MISSING_PARAMETER, CredentialRequestNames.Jwt));
            var didExtractor = _didExtractors.FirstOrDefault(e => user.Did.StartsWith($"did:{e.Type}"));
            if (didExtractor == null) return ProofValidationResult.Error(ErrorMessages.DID_METHOD_NOT_SUPPORTED);
            var didDocument = await didExtractor.Extract(user.Did, cancellationToken);
            var serializedJwt = request.Parameters[CredentialRequestNames.Jwt];
            var handler = new JsonWebTokenHandler();
            if (!handler.CanReadToken(serializedJwt)) return ProofValidationResult.Error(ErrorMessages.INVALID_PROOF_JWT);
            var jwt = handler.ReadJsonWebToken(serializedJwt);
            var isValidated = DidJwtValidator.Validate(jwt, didDocument);
            if (!isValidated) return ProofValidationResult.Error(ErrorMessages.INVALID_PROOF_SIG);
            return ProofValidationResult.Ok();
        }
    }
}
