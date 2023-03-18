// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.IdServer.Domains
{
    public class CertificateAuthority
    {
        public string Id { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public CertificateAuthoritySources Source { get; set; } = CertificateAuthoritySources.DB;
        public StoreLocation? StoreLocation { get; set; } = null;
        public StoreName? StoreName { get; set; } = null;
        public X509FindType? FindType { get; set; } = null;
        public string? FindValue { get; set; } = null;
        public string? Password { get; set; } = null;
        public string? PublicKey { get; set; } = null;
        public string? PrivateKey { get; set; } = null;
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public ICollection<ClientCertificate> ClientCertificates { get; set; } = new List<ClientCertificate>();
        public ICollection<Realm> Realms { get; set; } = new List<Realm>();
    }

    public enum CertificateAuthoritySources
    {
        DB = 0,
        CERTIFICATESTORE = 1
    }
}
