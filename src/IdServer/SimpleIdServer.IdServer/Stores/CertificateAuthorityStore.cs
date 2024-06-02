// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores
{
    public interface ICertificateAuthorityStore
    {
        Task<(CertificateAuthority, X509Certificate2)?> Get(string id, CancellationToken cancellationToken);
        X509Certificate2 Get(CertificateAuthority certificateAuthority);
    }

    public class CertificateAuthorityStore : ICertificateAuthorityStore
    {
        protected Dictionary<CertificateAuthoritySources, Func<CertificateAuthority, X509Certificate2>> _mappingTypeToBuiler = new Dictionary<CertificateAuthoritySources, Func<CertificateAuthority, X509Certificate2>>
        {
            { CertificateAuthoritySources.DB, ExtractFromDB },
            { CertificateAuthoritySources.CERTIFICATESTORE, ExtractFromCertificateStore }
        };
        private readonly ICertificateAuthorityRepository _repository;

        public CertificateAuthorityStore(ICertificateAuthorityRepository repository)
        {
            _repository = repository;
        }

        public async Task<(CertificateAuthority, X509Certificate2)?> Get(string id, CancellationToken cancellationToken)
        {
            var ca = await _repository.Get(id, cancellationToken);
            if (ca == null) return null;
            return (ca, _mappingTypeToBuiler[ca.Source](ca));
        }

        public X509Certificate2 Get(CertificateAuthority certificateAuthority) => _mappingTypeToBuiler[certificateAuthority.Source](certificateAuthority);

        protected static X509Certificate2 ExtractFromDB(CertificateAuthority ca) => X509Certificate2.CreateFromPem(ca.PublicKey, ca.PrivateKey);

        protected static X509Certificate2 ExtractFromCertificateStore(CertificateAuthority ca)
        {
            var store = new X509Store(ca.StoreName.Value, ca.StoreLocation.Value);
            store.Open(OpenFlags.ReadOnly);
            return store.Certificates.Find(ca.FindType.Value, ca.FindValue, true).First();
        }
    }
}
