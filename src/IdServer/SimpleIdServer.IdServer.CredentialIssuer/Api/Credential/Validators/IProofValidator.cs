// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
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

        public string ErrorMessage { get; private set; }

        public static ProofValidationResult Ok() => new(string.Empty);

        public static ProofValidationResult Error(string errorMessage) => new(errorMessage);
    }
}
