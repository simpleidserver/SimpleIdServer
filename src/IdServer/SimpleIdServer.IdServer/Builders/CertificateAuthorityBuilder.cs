// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.IdServer.Builders
{
    public class CertificateAuthorityBuilder
    {
        private readonly CertificateAuthority _ca;

        internal CertificateAuthorityBuilder(CertificateAuthority ca)
        {
            _ca = ca;
        }

        public static CertificateAuthorityBuilder Create(string subjectName, Realm realm = null, int numberOfDays = 365)
        {
            var ca = KeyGenerator.GenerateCertificateAuthority(subjectName, "password", numberOfDays);
            
            var pem = new PemResult(ca.ExportCertificatePem(), ca.GetRSAPrivateKey().ExportRSAPrivateKeyPem());
            var result = new CertificateAuthority
            {
                Id = Guid.NewGuid().ToString(),
                PrivateKey = pem.PrivateKey,
                PublicKey = pem.PublicKey,
                UpdateDateTime = DateTime.UtcNow,
                StartDateTime = ca.NotBefore,
                EndDateTime = ca.NotAfter,
                Source = CertificateAuthoritySources.DB,
                SubjectName = subjectName,
            };
            if (realm != null) result.Realms.Add(realm);
            return new CertificateAuthorityBuilder(result);
        }

        public CertificateAuthority Build() => _ca;
    }
}
