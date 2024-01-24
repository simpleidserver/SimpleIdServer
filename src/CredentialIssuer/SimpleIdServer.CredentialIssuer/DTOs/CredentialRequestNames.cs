// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.CredentialIssuer.DTOs
{
    public static class CredentialRequestNames
    {
        public const string Format = "format";
        public const string Proof = "proof";
        public const string ProofType = "proof_type";
        public const string Types = "types";
        public const string Jwt = "jwt";
        public const string CredentialIdentifier = "credential_identifier";
        public const string CredentialEncryptionJwk = "credential_encryption_jwk";
        public const string CredentialResponseEncryptionAlg = "credential_response_encryption_alg";
        public const string CredentialResponseEncryptionEnc = "credential_response_encryption_enc";
    }
}
