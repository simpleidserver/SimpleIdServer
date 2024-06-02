// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface ICertificateAuthorityRepository
{
    Task<CertificateAuthority> Get(string id, CancellationToken cancellationToken);
    Task<CertificateAuthority> Get(string realm, string id, CancellationToken cancellationToken);
    Task<SearchResult<CertificateAuthority>> Search(string realm, SearchRequest request, CancellationToken cancellationToken);
    void Add(CertificateAuthority certificateAuthority);
    void Delete(CertificateAuthority certificateAuthority);
    void Update(CertificateAuthority certificateAuthority);
}