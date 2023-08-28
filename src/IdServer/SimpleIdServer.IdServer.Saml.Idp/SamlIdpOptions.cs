// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;

namespace SimpleIdServer.IdServer.Saml.Idp;

public class SamlIdpOptions
{
    /// <summary>
    /// Sign the authentication request.
    /// </summary>
    public bool SignAuthnRequest { get; set; } = true;

    /// <summary>
    /// [Optional] Optional element identifying various kinds of contact personnel.
    /// </summary>
    public IEnumerable<ContactPerson> ContactPersons { get; set; } = new List<ContactPerson>
    {
        new ContactPerson(ContactTypes.Support)
        {
            Company = "LOKIT",
            EmailAddress = "agentsimpleidserver@gmail.com"
        }
    };

    public X509RevocationMode RevocationMode { get; set; } = X509RevocationMode.NoCheck;
    public X509CertificateValidationMode CertificateValidationMode { get; set; } = X509CertificateValidationMode.None;
}
