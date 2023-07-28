// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fido2NetLib;

namespace SimpleIdServer.IdServer.U2FClient
{
    public record EnrollResult
    {
        public EnrollResult(AttestationCertificateResult attestationCertificate, AuthenticatorAttestationRawResponse response, byte[] credentialId)
        {
            AttestationCertificate = attestationCertificate;
            Response = response;
            CredentialId = credentialId;
        }

        public AttestationCertificateResult AttestationCertificate { get; private set; } = null!;
        public AuthenticatorAttestationRawResponse Response { get; private set; } = null!;
        public byte[] CredentialId { get; private set; } = null!;
    }
}
