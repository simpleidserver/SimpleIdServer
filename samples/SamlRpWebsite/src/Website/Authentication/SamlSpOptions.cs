// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Security.Cryptography.X509Certificates;
using Website.Authentication;

namespace Microsoft.AspNetCore.Authentication;

public class SamlSpOptions : RemoteAuthenticationOptions
{
    public SamlSpOptions()
    {
        CallbackPath = new PathString("/saml2-signin");
        Events = new SamlSpEvents();
    }

    /// <summary>
    /// Unique identifier of the Service Provider (SP).
    /// </summary>
    public string? SPId { get; set; } = "urn:sp";

    /// <summary>
    /// Name of the Service Provider.
    /// </summary>
    public string? SPName { get; set; } = "SP";

    /// <summary>
    /// Metadata URL of the IdentityProvider.
    /// </summary>
    public string? IdpMetadataUrl { get; set; } = null;

    /// <summary>
    /// 
    /// </summary>
    public X509Certificate2? SigningCertificate { get; set; } = null;

    /// <summary>
    /// Gets or sets the SamlSpEvents used to handle authentication events.
    /// </summary>
    public new SamlSpEvents Events
    {
        get { return (SamlSpEvents)base.Events; }
        set { base.Events = value; }
    }
}
