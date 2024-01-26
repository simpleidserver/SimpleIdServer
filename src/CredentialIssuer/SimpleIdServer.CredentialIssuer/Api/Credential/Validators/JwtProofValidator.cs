// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.CredentialIssuer.Api.Credential.Validators
{
    /*
    public class JwtProofValidator : IProofValidator
    {
        private readonly IEnumerable<IIdentityDocumentExtractor> _didExtractors;
        private readonly ICredIssuerTokenHelper _credIssuerTokenHelper;

        public JwtProofValidator(IEnumerable<IIdentityDocumentExtractor> didExtractors, ICredIssuerTokenHelper credIssuerTokenHelper)
        {
            _didExtractors = didExtractors;
            _credIssuerTokenHelper = credIssuerTokenHelper;
        }

        public string Type => Constants.StandardProofTypes.Jwt;

        public async Task<ProofValidationResult> Validate(CredentialProofRequest request, User user, CancellationToken cancellationToken)
        {
            const string proofType = "openid4vci-proof+jwt";
            if (string.IsNullOrWhiteSpace(user.Did)) return ProofValidationResult.Error(ErrorMessages.USER_HAS_NO_DID);
            if (!request.Parameters.ContainsKey(CredentialRequestNames.Jwt)) return ProofValidationResult.Error(string.Format(ErrorMessages.MISSING_PARAMETER, CredentialRequestNames.Jwt));
            var didExtractor = _didExtractors.FirstOrDefault(e => user.Did.StartsWith($"did:{e.Type}"));
            if (didExtractor == null) return ProofValidationResult.Error(ErrorMessages.DID_METHOD_NOT_SUPPORTED);
            var didDocument = await didExtractor.Extract(user.Did, cancellationToken);
            var serializedJwt = request.Parameters[CredentialRequestNames.Jwt];
            var handler = new JsonWebTokenHandler();
            if (!handler.CanReadToken(serializedJwt)) return ProofValidationResult.Error(ErrorMessages.INVALID_PROOF_JWT);
            var jwt = handler.ReadJsonWebToken(serializedJwt);
            var isValidated = DidJwtValidator.Validate(jwt, didDocument, Did.Models.KeyPurposes.VerificationKey);
            if (!isValidated) return ProofValidationResult.Error(ErrorMessages.INVALID_PROOF_SIG);
            if (jwt.Typ != proofType) return ProofValidationResult.Error(string.Format(ErrorMessages.INVALID_PROOF_JWT_TYP, proofType));
            var cNonce = jwt.GetClaim(CredIssuerTokenResponseParameters.CNonce);
            double? expiresIn; 
            if (cNonce == null || (expiresIn = await (_credIssuerTokenHelper.GetCredentialNonce(cNonce.Value, cancellationToken))) == null) return ProofValidationResult.Error(ErrorMessages.INVALID_PROOF_C_NONCE);
            return ProofValidationResult.Ok(cNonce.Value, expiresIn.Value, didDocument);
        }
    }
    */
}
