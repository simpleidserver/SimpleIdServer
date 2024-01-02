// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using SimpleIdServer.Did;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Models;

namespace SimpleIdServer.Vc.Tests;

public class SecuredVerifiableCredentialFixture
{
    [Test]
    public void When_Secure_With_Ed25519_Then_Proof_Is_Added()
    {
        // ARRANGE
        var json = "{\r\n    \"@context\": [\r\n      \"https://www.w3.org/2018/credentials/v1\",\r\n      \"https://www.w3.org/2018/credentials/examples/v1\",\r\n      \"https://w3id.org/security/suites/jws-2020/v1\"\r\n    ],\r\n    \"id\": \"http://example.gov/credentials/3732\",\r\n    \"type\": [\"VerifiableCredential\", \"UniversityDegreeCredential\"],\r\n    \"issuer\": {\r\n      \"id\": \"https://example.com/issuer/123\"\r\n    },\r\n    \"issuanceDate\": \"2020-03-10T04:24:12.164Z\",\r\n    \"credentialSubject\": {\r\n      \"id\": \"did:example:456\",\r\n      \"degree\": {\r\n        \"type\": \"BachelorDegree\",\r\n        \"name\": \"Bachelor of Science and Arts\"\r\n      }\r\n    }\r\n}";
        var ed25119Sig = Ed25519SignatureKey.Generate();
        var identityDocument = DidDocumentBuilder.New("did", true)
            .AddAlsoKnownAs("didSubject")
            .AddController("didController")
            .AddEd25519VerificationKey2020VerificationMethod(ed25119Sig, "controller", VerificationMethodUsages.AUTHENTICATION)
            .Build();
        var vc = SecuredVerifiableCredential.New();

        // ACT
        var securedJson = vc.Secure(json, identityDocument, identityDocument.VerificationMethod.First().Id);

        // ASSERT
        Assert.IsNotNull(securedJson);
    }
}
