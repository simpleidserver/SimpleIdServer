// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains
{
    public class ClientCertificate
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string PublicKey { get; set; } = null!;
        public string PrivateKey { get; set; } = null!;
        public CertificateAuthority CertificateAuthority { get; set; }
    }
}
