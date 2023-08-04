// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.U2FClient
{
    public class AuthenticationParameter
    {
        public string Rp { get; set; } = "localhost";
        public string Origin { get; set; } = "https://localhost:5001";
        public byte[] CredentialId { get; set; } = null!;
        public byte[] Challenge { get; set; } = null!;
        public uint Signcount { get; set; }
        public AttestationCertificateResult Certificate { get; set; } = null!;
    }
}
