// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SqlSugar;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("CertificateAuthorities")]
public class SugarCertificateAuthority
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = null!;
    public string SubjectName { get; set; } = null!;
    public CertificateAuthoritySources Source { get; set; }
    public StoreLocation? StoreLocation { get; set; } = null;
    public StoreName? StoreName { get; set; } = null;
    public X509FindType? FindType { get; set; } = null;
    public string? FindValue { get; set; } = null;
    public string? PublicKey { get; set; } = null;
    public string? PrivateKey { get; set; } = null;
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarClientCertificate.CertificateAuthorityId))]
    public List<SugarClientCertificate> ClientCertificates { get; set; }
    [Navigate(typeof(SugarCertificateAuthorityRealm), nameof(SugarCertificateAuthorityRealm.CertificateAuthoritiesId), nameof(SugarCertificateAuthorityRealm.RealmsName))]
    public List<SugarRealm> Realms { get; set; }

    public CertificateAuthority ToDomain()
    {
        return new CertificateAuthority
        {
            Id = Id,
            SubjectName = SubjectName,
            Source = Source,
            StoreLocation = StoreLocation,
            StoreName = StoreName,
            FindType = FindType,
            FindValue = FindValue,
            PublicKey = PublicKey,
            PrivateKey = PrivateKey,
            StartDateTime = StartDateTime,
            EndDateTime = EndDateTime,
            UpdateDateTime = UpdateDateTime,
            Realms = Realms.Select(r => r.ToDomain()).ToList(),
            ClientCertificates = ClientCertificates == null ? new List<ClientCertificate>() : ClientCertificates.Select(c => c.ToDomain()).ToList()
        };
    }
}
