// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace SimpleIdServer.IdServer.U2FClient
{
    public class AttestationCertificateResult
    {
        public AttestationCertificateResult(X509Certificate2 attestationCertificate, ECDsa privateKey)
        {
            AttestationCertificate = attestationCertificate;
            PrivateKey = privateKey;
        }

        public X509Certificate2 AttestationCertificate { get; private set; }
        public ECDsa PrivateKey { get; private set; }
    }
}
