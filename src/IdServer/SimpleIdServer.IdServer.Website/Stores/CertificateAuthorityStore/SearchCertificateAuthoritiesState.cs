// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.CertificateAuthorityStore
{
    [FeatureState]
    public record SearchCertificateAuthoritiesState
    {
        public SearchCertificateAuthoritiesState() { }

        public SearchCertificateAuthoritiesState(bool isLoading, int count, IEnumerable<CertificateAuthority> certificateAuthorities)
        {
            CertificateAuthorities = certificateAuthorities.Select(c => new SelectableCertificateAuthority(c));
            Count = count;
            IsLoading = isLoading;
        }

        public IEnumerable<SelectableCertificateAuthority>? CertificateAuthorities { get; set; } = null;
        public int Count { get; set; } = 0;
        public bool IsLoading { get; set; } = true;
    }

    public class SelectableCertificateAuthority
    {
        public SelectableCertificateAuthority(CertificateAuthority certificateAuthority)
        {
            Value = certificateAuthority;
        }

        public bool IsSelected { get; set; } = false;
        public bool IsNew { get; set; } = false;
        public CertificateAuthority Value { get; set; }
    }
}
