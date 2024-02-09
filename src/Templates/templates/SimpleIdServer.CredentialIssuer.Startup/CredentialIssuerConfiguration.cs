// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.CredentialIssuer.Builders;
using SimpleIdServer.CredentialIssuer.CredentialFormats;
using SimpleIdServer.CredentialIssuer.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.CredentialIssuer.Startup;

public class CredentialIssuerConfiguration
{
    public static List<CredentialConfiguration> CredentialConfigurations => new List<CredentialConfiguration>
    {
        CredentialConfigurationBuilder
            .New(LdpVcFormatter.FORMAT, "UniversityDegree", "https://www.w3.org/2018/credentials/examples/v1", "https://www.w3.org/2018/credentials", scope: "university_degree")
            .AddClaim("given_name", "GivenName", (cb) =>
            {
                cb.AddTranslation("Given Name", "en-US");
            })
            .AddClaim("family_name", "FamilyName", (cb) => 
            {
                cb.AddTranslation("Surname", "en-US");
            })
            .AddClaim("degree.type", "DegreeType", (cb) =>
            {
                cb.AddTranslation("Type of degree", "en-US");
            })
            .AddClaim("degree.name", "DegreeName", (cb) =>
            {
                cb.AddTranslation("Name of degree", "en-US");
            })
            .AddDisplay("University Credential", "en-US", "https://university.example.edu/public/logo.png", "A square logo of a university", null,"#12107c", "#acd2b1")
            .Build()
    };
}
