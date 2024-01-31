// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using SimpleIdServer.Did;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Encoders;
using SimpleIdServer.Did.Jwt;
using SimpleIdServer.Did.Models;
using SimpleIdServer.Vc.CredentialFormats.Serializers;

namespace SimpleIdServer.Vc.Tests;

public class JwtVerifiableCredentialFixture
{
    private const string _json = "{\r\n    \"@context\": [\r\n      \"https://www.w3.org/2018/credentials/v1\",\r\n      \"https://www.w3.org/2018/credentials/examples/v1\",\r\n      \"https://w3id.org/security/suites/jws-2020/v1\"\r\n    ],\r\n    \"id\": \"http://example.gov/credentials/3732\",\r\n    \"type\": [\"VerifiableCredential\", \"UniversityDegreeCredential\"],\r\n   \"issuanceDate\": \"2020-03-10T04:24:12.164Z\",\r\n    \"credentialSubject\": {\r\n      \"id\": \"did:example:456\",\r\n      \"degree\": {\r\n        \"type\": \"BachelorDegree\",\r\n        \"name\": \"Bachelor of Science and Arts\"\r\n      }\r\n    }\r\n}";

    #region Ed25519VerificationKey2020

    [Test]
    public void When_Secure_Jwt_VerifiableCredentials_With_Random_Ed25519VerificationKey2020_VerificationKey_Then_Proof_Is_Valid()
    {
        // ARRANGE
        var ed25119Sig = Ed25519SignatureKey.Generate();
        var identityDocument = DidDocumentBuilder.New("did")
            .AddAlsoKnownAs("didSubject")
            .AddController("didController")
            .AddVerificationMethod(Ed25519VerificationKey2020Standard.TYPE, ed25119Sig, "controller", VerificationMethodUsages.ASSERTION_METHOD, includePrivateKey: true)
            .Build();
        var vc = SecuredVerifiableCredential.New();
        var handler = DidJsonWebTokenHandler.New();

        // ACT
        var securedJson = vc.SecureJwt("http://localhost:5001",
            identityDocument, 
            identityDocument.VerificationMethod.First().Id,
            new W3CVerifiableCredentialJsonSerializer().Deserialize(_json));
        var isSignatureValid = handler.CheckJwt(securedJson, identityDocument);

        // ASSERT
        Assert.IsNotNull(securedJson);
        Assert.IsTrue(isSignatureValid);
    }

    #endregion

    #region JsonWebKey2020

    [Test]
    public void When_Secure_Jwt_VerifiableCredentials_With_Random_JsonWebKey2020_VerificationKey_Then_Proof_Is_Valid()
    {
        // ARRANGE
        var es256Sig = ES256SignatureKey.Generate();
        var identityDocument = DidDocumentBuilder.New("did")
            .AddAlsoKnownAs("didSubject")
            .AddController("didController")
            .AddVerificationMethod(JsonWebKey2020Standard.TYPE, es256Sig, "controller", VerificationMethodUsages.ASSERTION_METHOD, includePrivateKey: true)
            .Build();
        var vc = SecuredVerifiableCredential.New();
        var handler = DidJsonWebTokenHandler.New();

        // ACT
        var securedJson = vc.SecureJwt("http://localhost:5001",
            identityDocument,
            identityDocument.VerificationMethod.First().Id,
            new W3CVerifiableCredentialJsonSerializer().Deserialize(_json));
        var isSignatureValid = handler.CheckJwt(securedJson, identityDocument);

        // ASSERT
        Assert.IsNotNull(securedJson);
        Assert.IsTrue(isSignatureValid);
    }

    #endregion
}
