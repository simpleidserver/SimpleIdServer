// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.IdServer.Website.Stores.CertificateAuthorityStore
{
    [FeatureState]
    public record CertificateAuthorityState
    {
        public CertificateAuthorityState() { }

        public CertificateAuthorityState(CertificateAuthority certificateAuthority, X509Certificate2 certificate, bool isLoading)
        {
            CertificateAuthority = certificateAuthority;
            Certificate = certificate;
            IsLoading = isLoading;
        }

        public CertificateAuthority CertificateAuthority { get; set; }
        public X509Certificate2 Certificate { get; set; }
        public bool IsLoading { get; set; } = true;
    }
}
