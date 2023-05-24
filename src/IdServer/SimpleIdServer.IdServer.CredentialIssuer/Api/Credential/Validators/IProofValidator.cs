// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Models;
using SimpleIdServer.IdServer.Domains;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.CredentialIssuer.Api.Credential.Validators
{
    public interface IProofValidator
    {
        string Type { get; }
        Task<ProofValidationResult> Validate(CredentialProofRequest request, User user, CancellationToken cancellationToken);
    }

    public record ProofValidationResult
    {
        private ProofValidationResult(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public ProofValidationResult(string cNonce, double cNonceExpiresIn, IdentityDocument identityDocument)
        {
            CNonce = cNonce;
            CNonceExpiresIn = cNonceExpiresIn;
            IdentityDocument = identityDocument;
        }

        public string ErrorMessage { get; private set; }
        public string CNonce { get; private set; }
        public double CNonceExpiresIn { get; private set; }
        public IdentityDocument IdentityDocument { get; private set; }

        public static ProofValidationResult Ok(string cNonce, double cNonceExpiresIn, IdentityDocument identityDocument) => new(cNonce, cNonceExpiresIn, identityDocument);

        public static ProofValidationResult Error(string errorMessage) => new(errorMessage);
    }
}
