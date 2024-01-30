// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.Did;
using SimpleIdServer.IdServer.CredentialIssuer;
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using SimpleIdServer.Vc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.CredentialIssuer.Api.Credential.Validators;

public class JwtKeyProofTypeValidator : IKeyProofTypeValidator
{
    private readonly IEnumerable<IDidResolver> _didResolvers;

    public JwtKeyProofTypeValidator(IEnumerable<IDidResolver> didResolvers)
    {
        _didResolvers = didResolvers;
    }

    public string Type => "jwt";

    public async Task<KeyProofTypeValidationResult> Validate(CredentialProofRequest request, CancellationToken cancellationToken)
    {
        const string type = "openid4vci-proof+jwt";
        const string none = "none";
        if (string.IsNullOrWhiteSpace(request.Jwt)) return KeyProofTypeValidationResult.Error(string.Format(ErrorMessages.MISSING_PARAMETER, CredentialRequestNames.Jwt));
        var handler = new JsonWebTokenHandler();
        if (!handler.CanReadToken(request.Jwt)) return KeyProofTypeValidationResult.Error(ErrorMessages.INVALID_PROOF_JWT);
        var jwt = handler.ReadJsonWebToken(request.Jwt);
        if (jwt.Typ != type) return KeyProofTypeValidationResult.Error(string.Format(ErrorMessages.INVALID_PROOF_JWT_TYP, type));
        if (jwt.Alg == none) return KeyProofTypeValidationResult.Error(string.Format(ErrorMessages.INVALID_PROOF_JWT_ALG, none));
        if (string.IsNullOrWhiteSpace(jwt.Kid)) return KeyProofTypeValidationResult.Error(ErrorMessages.MISSING_PROOF_JWT_KID);
        try
        {
            var did = DidExtractor.Extract(jwt.Kid);
            var resolver = _didResolvers.SingleOrDefault(r => r.Method == did.Method);
            if (resolver == null) return KeyProofTypeValidationResult.Error(string.Format(ErrorMessages.UNSUPPORTED_DID_METHOD, did.Method));
            var result = await resolver.Resolve(jwt.Kid, cancellationToken);
            if (!SecuredVerifiableCredential.New().CheckJwt(request.Jwt, result)) return KeyProofTypeValidationResult.Error(ErrorMessages.INVALID_PROOF_SIG);
            string nonce = null;
            var nonceClaim = jwt.Claims.SingleOrDefault(c => c.Type == "nonce");
            if (nonceClaim != null)
            {
                nonce = nonceClaim.Value;
            }

            return KeyProofTypeValidationResult.Ok(nonce);
        }
        catch(Exception ex)
        {
            return KeyProofTypeValidationResult.Error(ex.Message);
        }
    }
}
