// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains
{
    public class ClientCertificate
    {
        public string Id { get; set; } = null!;
        public string PublicKey { get; set; } = null!;
        public string PrivateKey { get; set; } = null!;
        public CertificateAuthority CertificateAuthority { get; set; }
    }
}
