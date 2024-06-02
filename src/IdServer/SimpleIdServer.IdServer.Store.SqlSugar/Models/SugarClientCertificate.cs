// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("ClientCertificate")]
public class SugarClientCertificate
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public DateTime CreateDateTime { get; set; }
    public string PublicKey { get; set; } = null!;
    public string PrivateKey { get; set; } = null!;
    public string CertificateAuthorityId { get; set; }
    [Navigate(NavigateType.ManyToOne, nameof(CertificateAuthorityId))]
    public CertificateAuthority CertificateAuthority { get; set; }

    public ClientCertificate ToDomain()
    {
        return new ClientCertificate
        {
            Id = Id,
            Name = Name,
            StartDateTime = StartDateTime,
            EndDateTime = EndDateTime,
            CreateDateTime = CreateDateTime,
            PublicKey = PublicKey,
            PrivateKey = PrivateKey
        };
    }
}
