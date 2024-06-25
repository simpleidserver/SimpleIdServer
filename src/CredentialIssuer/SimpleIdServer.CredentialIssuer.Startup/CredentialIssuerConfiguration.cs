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
            .New(JwtVcJsonFormatter.FORMAT, "UniversityDegree", "https://www.w3.org/2018/credentials/examples/v1", "https://www.w3.org/2018/credentials", scope: "university_degree")
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
            .Build(),
        CredentialConfigurationBuilder
            .New(JwtVcJsonFormatter.FORMAT, "CTWalletSameAuthorisedInTime", "https://www.w3.org/2018/credentials/examples/v1", "https://www.w3.org/2018/credentials", additionalTypes: new List<string> { "VerifiableAttestation" }, isDeferred: false)
            .SetSchema("https://api-pilot.ebsi.eu/trusted-schemas-registry/v2/schemas/z3MgUFUkb722uq4x3dv5yAJmnNmzDFeK5UC8x83QoeLJM", "FullJsonSchemaValidator2021")
            .Build(),
        CredentialConfigurationBuilder
            .New(JwtVcJsonFormatter.FORMAT, "CTWalletSameAuthorisedDeferred", "https://www.w3.org/2018/credentials/examples/v1", "https://www.w3.org/2018/credentials", additionalTypes: new List<string> { "VerifiableAttestation" }, isDeferred: true)
            .SetSchema("https://api-pilot.ebsi.eu/trusted-schemas-registry/v2/schemas/z3MgUFUkb722uq4x3dv5yAJmnNmzDFeK5UC8x83QoeLJM", "FullJsonSchemaValidator2021")
            .Build()
    };

    // EBSI - DID : did:key:z2dmzD81cgPx8Vki7JbuuMmFYrWPgYoytykUZ3eyqht1j9Kboj7g9PfXJxbbs4KYegyr7ELnFVnpDMzbJJDDNZjavX6jvtDmALMbXAGW67pdTgFea2FrGGSFs8Ejxi96oFLGHcL4P6bjLDPBJEvRRHSrG4LsPne52fczt2MWjHLLJBvhAC
    public static List<UserCredentialClaim> CredentialClaims => new List<UserCredentialClaim>
    {
        UserCredentialClaimBuilder.Build("administrator", "DegreeName", "Master degree"),
        UserCredentialClaimBuilder.Build("administrator", "GivenName", "SimpleIdServer")
    };
}
