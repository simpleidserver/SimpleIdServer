// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Org.BouncyCastle.Crypto.Agreement;
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
            .AddDisplay("University Credential", "en-US", "https://img.freepik.com/premium-vector/logo-university-name-logo-company-called-university_516670-732.jpg", "A square logo of a university", null,"#12107c", "#acd2b1")
            .Build()
    };

    public static List<UserCredentialClaim> CredentialClaims => new List<UserCredentialClaim>
    {
        UserCredentialClaimBuilder.Build("administrator", "DegreeName", "Master degree"),
        UserCredentialClaimBuilder.Build("administrator", "GivenName", "SimpleIdServer")
    };
}
