// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using SimpleIdServer.FastFed.Authentication.Saml;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.FastFed.ApplicationProvider.Authentication.Saml;

public class SamlAuthenticationOptions
{
    public string SpId { get; set; } = "samlApplicationProvider";
    public string SamlMetadataUri { get; set; }
    public X509Certificate2 SigningCertificate { get; set; }
    public HttpClientHandler BackchannelHttpHandler { get; set; }
    public IEnumerable<ContactPerson> ContactPersons { get; set; } = new List<ContactPerson>
    {
        new ContactPerson(ContactTypes.Technical)
        {
            Company = "SimpleIdServer",
            EmailAddress = "agentsimpleidserver@gmail.com"
        }
    };
    public SamlEntrepriseMappingsResult Mappings { get; set; }
    public int? CacheSamlAuthProvidersInSeconds { get; set; }
}
