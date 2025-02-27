// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;
using SimpleIdServer.Did;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Crypto.Multicodec;
using SimpleIdServer.Did.Encoders;
using SimpleIdServer.Did.Models;
using SimpleIdServer.Vc.Models;
using System.Text.Json;

namespace SimpleIdServer.Vc.Tests;

public class SecuredVerifiableCredentialFixture
{
    private const string _json = "{\r\n    \"@context\": [\r\n      \"https://www.w3.org/2018/credentials/v1\",\r\n      \"https://www.w3.org/2018/credentials/examples/v1\",\r\n      \"https://w3id.org/security/suites/jws-2020/v1\"\r\n    ],\r\n    \"id\": \"http://example.gov/credentials/3732\",\r\n    \"type\": [\"VerifiableCredential\", \"UniversityDegreeCredential\"],\r\n    \"issuer\": \"https://example.com/issuer/123\", \r\n    \"issuanceDate\": \"2020-03-10T04:24:12.164Z\",\r\n    \"credentialSubject\": {\r\n      \"id\": \"did:example:456\",\r\n      \"degree\": {\r\n        \"type\": \"BachelorDegree\",\r\n        \"name\": \"Bachelor of Science and Arts\"\r\n      }\r\n    }\r\n}";

    #region Ed25519VerificationKey2020

    [Test]
    public void When_Secure_VerifiableCredentials_With_Random_Ed25519VerificationKey2020_VerificationKey_Then_Proof_Is_Valid()
    {
        // ARRANGE
        var ed25119Sig = Ed25519SignatureKey.Generate();
        var identityDocument = DidDocumentBuilder.New("did")
            .AddAlsoKnownAs("didSubject")
            .AddController("didController")
            .AddVerificationMethod(Ed25519VerificationKey2020Standard.TYPE, ed25119Sig, "controller", VerificationMethodUsages.AUTHENTICATION, includePrivateKey: true)
            .Build();
        var vc = SecuredDocument.New();

        // ACT
        var credential = JsonSerializer.Deserialize<W3CVerifiableCredential>(_json);
        vc.Secure(credential, identityDocument, identityDocument.VerificationMethod.First().Id);
        var isSignatureValid = vc.Check(credential, identityDocument);

        // ASSERT
        Assert.That(credential != null);
        Assert.That(credential.Proof != null);
        Assert.That(isSignatureValid);
    }

    [Test]
    public void When_Secure_VerifiableCredentials_With_Static_Ed25519VerificationKey2020_VerificationKey_Then_Proof_Is_Valid()
    {
        // Test vector : https://w3c.github.io/vc-di-eddsa/#representation-ed25519signature2020
        // ARRANGE
        string json = "{\r\n    \"@context\": [\r\n        \"https://www.w3.org/ns/credentials/v2\",\r\n        \"https://www.w3.org/ns/credentials/examples/v2\"\r\n    ],\r\n    \"id\": \"urn:uuid:58172aac-d8ba-11ed-83dd-0b3aef56cc33\",\r\n    \"type\": [\"VerifiableCredential\", \"AlumniCredential\"],\r\n    \"name\": \"Alumni Credential\",\r\n    \"description\": \"A minimum viable example of an Alumni Credential.\",\r\n    \"issuer\": \"https://vc.example/issuers/5678\",\r\n    \"validFrom\": \"2023-01-01T00:00:00Z\",\r\n    \"credentialSubject\": {\r\n        \"id\": \"did:example:abcdefgh\",\r\n        \"alumniOf\": \"The School of Examples\"\r\n    }\r\n}";
        var publicKeybase = "z6MkrJVnaZkeFzdQyMZu1cgjg7k1pZZ6pvBQ7XJPt4swbTQ2";
        var privateKeybase = "z3u2en7t5LR2WtQH5PfFqMqwVHBeXouLzo6haApm8XHqvjxq";
        var factory = MulticodecSerializerFactory.Build();
        var asymKey = factory.Deserialize(publicKeybase, privateKeybase);
        var identityDocument = DidDocumentBuilder.New("did")
            .AddAlsoKnownAs("didSubject")
            .AddController("didController")
            .AddVerificationMethod(Ed25519VerificationKey2020Standard.TYPE, asymKey, "controller", VerificationMethodUsages.AUTHENTICATION, callback: c =>
            {
                c.Id = "https://vc.example/issuers/5678#z6MkrJVnaZkeFzdQyMZu1cgjg7k1pZZ6pvBQ7XJPt4swbTQ2";
            }, includePrivateKey: true)
            .Build();
        var vc = SecuredDocument.New();

        // ACT
        var credential = JsonSerializer.Deserialize<W3CVerifiableCredential>(json);
        vc.Secure(credential, identityDocument, identityDocument.VerificationMethod.First().Id, creationDateTime: DateTime.Parse("2023-02-24T23:36:38Z", null, System.Globalization.DateTimeStyles.RoundtripKind));
        var isSignatureValid = vc.Check(credential, identityDocument);

        // ASSERT
        Assert.That(vc != null);
        Assert.That(isSignatureValid);
        Assert.That(credential.Proof != null);
        var proofValue = credential.Proof["proofValue"].ToString();
        Assert.That("zW3gwNq5uhWFj9UvZpLW57FqniTW21oKQ1JavgTtmarkPFekN4zEH3PzGBoCCG6CiLm9LzZiSWTKjPQyhFV44hD9" == proofValue);
    }

    #endregion

    #region JsonWebKey2020

    [Test]
    public void When_Secure_VerifiableCredentials_With_Random_ED25519_JsonWebKey2020_VerificationKey_Then_Proof_Is_Valid()
    {
        // ARRANGE
        var ed25119Sig = Ed25519SignatureKey.Generate();
        var identityDocument = DidDocumentBuilder.New("did")
            .AddAlsoKnownAs("didSubject")
            .AddController("didController")
            .AddVerificationMethod(JsonWebKey2020Standard.TYPE, ed25119Sig, "controller", VerificationMethodUsages.AUTHENTICATION, includePrivateKey: true)
            .Build();
        var vc = SecuredDocument.New();

        // ACT
        var credential = JsonSerializer.Deserialize<W3CVerifiableCredential>(_json);
        vc.Secure(credential, identityDocument, identityDocument.VerificationMethod.First().Id);
        var isSignatureValid = vc.Check(credential, identityDocument);

        // ASSERT
        Assert.That(credential != null);
        Assert.That(credential.Proof != null);
        Assert.That(isSignatureValid);
    }

    [Test]
    public void When_Secure_VerifiableCredentials_With_Static_ED25519_JsonWebKey2020_VerificationKey_Then_Proof_Is_Valid()
    {
        // ARRANGE
        var json = "{\r\n    \"@context\": [\r\n      \"https://www.w3.org/2018/credentials/v1\",\r\n      \"https://www.w3.org/2018/credentials/examples/v1\",\r\n      \"https://w3id.org/security/suites/jws-2020/v1\"\r\n    ],\r\n    \"id\": \"http://example.gov/credentials/3732\",\r\n    \"type\": [\"VerifiableCredential\", \"UniversityDegreeCredential\"],\r\n    \"issuer\": \"did:example:123\" ,\r\n    \"issuanceDate\": \"2020-03-10T04:24:12.164Z\",\r\n    \"credentialSubject\": {\r\n      \"id\": \"did:example:456\",\r\n      \"degree\": {\r\n        \"type\": \"BachelorDegree\",\r\n        \"name\": \"Bachelor of Science and Arts\"\r\n      }\r\n    }\r\n  }";
        var x = "CV-aGlld3nVdgnhoZK0D36Wk-9aIMlZjZOK2XhPMnkQ";
        var d = "m5N7gTItgWz6udWjuqzJsqX-vksUnxJrNjD5OilScBc";
        var xPayload = Base64UrlEncoder.DecodeBytes(x);
        var dPayload = Base64UrlEncoder.DecodeBytes(d);
        var ed25119Sig = Ed25519SignatureKey.From(xPayload, dPayload);
        var identityDocument = DidDocumentBuilder.New("did")
            .AddController("didController")
            .AddVerificationMethod(JsonWebKey2020Standard.TYPE, ed25119Sig, "controller", VerificationMethodUsages.AUTHENTICATION, includePrivateKey: true)
            .Build();
        var vc = SecuredDocument.New();

        // ACT
        var credential = JsonSerializer.Deserialize<W3CVerifiableCredential>(json);
        vc.Secure(credential, identityDocument, identityDocument.VerificationMethod.First().Id);

        Assert.That(credential != null);
    }

    #endregion
}