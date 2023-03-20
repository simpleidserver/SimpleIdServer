// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.CertificateAuthorityStore
{
    [FeatureState]
    public record UpdateCertificateAuthorityState
    {
        public UpdateCertificateAuthorityState() { }

        public UpdateCertificateAuthorityState(bool isUpdating, CertificateAuthority certificateAuthority)
        {
            IsUpdating = isUpdating;
            CertificateAuthority= certificateAuthority;
        }

        public bool IsUpdating { get; set; }
        public CertificateAuthority CertificateAuthority { get; set; }
        public string ErrorMessage { get; set; }
    }
}
