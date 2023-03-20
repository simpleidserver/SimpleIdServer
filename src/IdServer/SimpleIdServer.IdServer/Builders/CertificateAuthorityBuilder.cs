// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System;
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

        public static CertificateAuthorityBuilder Import(X509Certificate2 certificate, StoreLocation storeLocation, StoreName storeName, X509FindType findType, string findValue, Realm realm = null)
        {
            var result = new CertificateAuthority
            {
                Id = Guid.NewGuid().ToString(),
                UpdateDateTime = DateTime.UtcNow,
                StartDateTime = certificate.NotBefore,
                EndDateTime = certificate.NotAfter,
                Source = CertificateAuthoritySources.CERTIFICATESTORE,
                SubjectName = certificate.SubjectName.Name,
                StoreLocation = storeLocation,
                StoreName = storeName,
                FindType = findType,
                FindValue = findValue
            };
            if (realm != null) result.Realms.Add(realm);
            return new CertificateAuthorityBuilder(result);
        }

        public CertificateAuthority Build() => _ca;
    }
}
