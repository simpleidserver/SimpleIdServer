// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;

namespace SimpleIdServer.IdServer.Saml.Sp;

public class SamlSpOptions : RemoteAuthenticationOptions
{
    public SamlSpOptions()
    {
        CallbackPath = new PathString("/signin-saml2");
        Events = new SamlSpEvents();
    }

    /// <summary>
    /// Unique identifier of the Service Provider (SP).
    /// </summary>
    public string? SPId { get; set; } = "urn:sp";

    /// <summary>
    /// Unique identifier of the Identity Provider (Idp)
    /// </summary>
    public string? IdpId { get; set; } = "https://localhost:5001";

    /// <summary>
    /// Name of the Service Provider.
    /// </summary>
    public string? SPName { get; set; } = "SP";

    /// <summary>
    /// Metadata URL of the IdentityProvider.
    /// </summary>
    public string? IdpMetadataUrl { get; set; } = "https://localhost:5001/master/saml/metadata";

    /// <summary>
    /// Certificate used to sign the request.
    /// </summary>
    public X509Certificate2? SigningCertificate { get; set; } = null;

    /// <summary>
    /// List of contact person.
    /// </summary>
    public IEnumerable<ContactPerson> ContactPersons { get; set; } = new List<ContactPerson>
    {
        new ContactPerson(ContactTypes.Technical)
        {
            Company = "SimpleIdServer",
            EmailAddress = "agentsimpleidserver@gmail.com"

        }
    };
    public X509RevocationMode RevocationMode { get; set; } = X509RevocationMode.NoCheck;
    public X509CertificateValidationMode CertificateValidationMode { get; set; } = X509CertificateValidationMode.None;

    /// <summary>
    /// Gets or sets the SamlSpEvents used to handle authentication events.
    /// </summary>
    public new SamlSpEvents Events
    {
        get { return (SamlSpEvents)base.Events; }
        set { base.Events = value; }
    }
}
