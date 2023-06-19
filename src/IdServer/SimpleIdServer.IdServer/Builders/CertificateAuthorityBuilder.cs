// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System;
using System.Security.Cryptography;
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

        public static CertificateAuthorityBuilder Create(string subjectName, Domains.Realm realm = null, int numberOfDays = 365)
        {
            var caResult = KeyGenerator.GenerateCertificateAuthority(subjectName, numberOfDays); 
            var privateKey = new string(PemEncoding.Write("PRIVATE KEY", caResult.Item2.ExportPkcs8PrivateKey()));
            var pem = new PemResult(caResult.Item1.ExportCertificatePem(), privateKey);
            var result = new CertificateAuthority
            {
                Id = Guid.NewGuid().ToString(),
                PrivateKey = pem.PrivateKey,
                PublicKey = pem.PublicKey,
                UpdateDateTime = DateTime.UtcNow,
                StartDateTime = caResult.Item1.NotBefore,
                EndDateTime = caResult.Item1.NotAfter,
                Source = CertificateAuthoritySources.DB,
                SubjectName = subjectName,
            };
            if (realm != null) result.Realms.Add(realm);
            return new CertificateAuthorityBuilder(result);
        }

        public static CertificateAuthorityBuilder Import(X509Certificate2 certificate, StoreLocation storeLocation, StoreName storeName, X509FindType findType, string findValue, Domains.Realm realm = null)
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
