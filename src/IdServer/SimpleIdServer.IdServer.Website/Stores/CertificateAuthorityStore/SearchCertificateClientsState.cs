// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.IdServer.Website.Stores.CertificateAuthorityStore
{
    [FeatureState]
    public record SearchCertificateClientsState
    {
        public SearchCertificateClientsState() { }

        public SearchCertificateClientsState(bool isLoading, int count, IEnumerable<ClientCertificate> certificateAuthorities)
        {
            ClientCertificates = certificateAuthorities.Select(c => new SelectableClientCertificate(c, X509Certificate2.CreateFromPem(c.PublicKey, c.PrivateKey)));
            Count = count;
            IsLoading = isLoading;
        }

        public IEnumerable<SelectableClientCertificate> ClientCertificates { get; set; } = new List<SelectableClientCertificate>();
        public int Count { get; set; } = 0;
        public bool IsLoading { get; set; } = true;
    }

    public class SelectableClientCertificate
    {
        public SelectableClientCertificate(ClientCertificate certificateAuthority, X509Certificate2 certificate)
        {
            Value = certificateAuthority;
            Certificate = certificate;
        }

        public bool IsSelected { get; set; } = false;
        public bool IsNew { get; set; } = false;
        public ClientCertificate Value { get; set; }
        public X509Certificate2 Certificate { get; set; }
    }
}
