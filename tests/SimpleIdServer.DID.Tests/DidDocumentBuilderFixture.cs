// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using SimpleIdServer.Did;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Encoders;
using SimpleIdServer.Did.Models;
using System.Text.Json.Nodes;

namespace SimpleIdServer.DID.Tests
{
    public class DidDocumentBuilderFixture
    {
        [Test]
        public void When_Build_IdentityDocument_With_Ed25519VerificationKey2020VerificationMethod_Then_DocumentIsCorrect()
        {
            // https://www.w3.org/TR/did-spec-registries/#verification-method-types
            // ARRANGE
            var ed25119Sig = Ed25519SignatureKey.Generate();
            var identityDocument = DidDocumentBuilder.New("did")
                .AddAlsoKnownAs("didSubject")
                .AddController("didController")
                .AddVerificationMethod(Ed25519VerificationKey2020Standard.TYPE, ed25119Sig, "controller", VerificationMethodUsages.AUTHENTICATION)
                .Build();

            // ACT
            var json = identityDocument.Serialize();
            var contextLst = JsonObject.Parse(json)["@context"] as JsonArray;

            // ASSERT
            Assert.IsNotNull(json);
            Assert.That(identityDocument.Id, Is.EqualTo("did"));
            Assert.That(contextLst.ElementAt(0).ToString(), Is.EqualTo("https://www.w3.org/ns/did/v1"));
            Assert.That(contextLst.ElementAt(1).ToString(), Is.EqualTo("https://w3id.org/security/suites/ed25519-2020/v1"));
            Assert.That(identityDocument.AlsoKnownAs.First(), Is.EqualTo("didSubject"));
            Assert.That(identityDocument.Controller.ToString(), Is.EqualTo("didController"));
            Assert.That(identityDocument.VerificationMethod.ElementAt(0).Type, Is.EqualTo("Ed25519VerificationKey2020"));
            Assert.IsNotNull(identityDocument.VerificationMethod.ElementAt(0).PublicKeyMultibase);
        }

        [Test]
        public void When_Build_IdentityDocument_With_JsonWebKey2020VerificationMethods_Then_Document_Is_Correct()
        {
            // ARRANGE
            var ed25119Sig = Ed25519SignatureKey.Generate();
            var es256K = ES256KSignatureKey.Generate();
            var es256 = ES256SignatureKey.Generate();
            var es384 = ES384SignatureKey.Generate();

            // ACT
            var identityDocument = DidDocumentBuilder.New("did")
                .AddAlsoKnownAs("didSubject")
                .AddController("didController")
                .AddVerificationMethod(JsonWebKey2020Standard.TYPE, ed25119Sig, "controller", VerificationMethodUsages.AUTHENTICATION)
                .AddVerificationMethod(JsonWebKey2020Standard.TYPE, es256K, "controller", VerificationMethodUsages.AUTHENTICATION)
                .AddVerificationMethod(JsonWebKey2020Standard.TYPE, es256, "controller", VerificationMethodUsages.AUTHENTICATION)
                .AddVerificationMethod(JsonWebKey2020Standard.TYPE, es384, "controller", VerificationMethodUsages.AUTHENTICATION)
                .Build();

            // ASSERT
            var json = identityDocument.Serialize();
            var contextLst = JsonObject.Parse(json)["@context"] as JsonArray;

            // ASSERT
            Assert.That(identityDocument.Id, Is.EqualTo("did"));
            Assert.That(contextLst.ElementAt(0).ToString(), Is.EqualTo("https://www.w3.org/ns/did/v1"));
            Assert.That(contextLst.ElementAt(1).ToString(), Is.EqualTo("https://w3id.org/security/suites/jws-2020/v1"));
            Assert.That(identityDocument.AlsoKnownAs.First(), Is.EqualTo("didSubject"));
            Assert.That(identityDocument.Controller.ToString(), Is.EqualTo("didController"));
            Assert.That(identityDocument.VerificationMethod.ElementAt(0).Type, Is.EqualTo("JsonWebKey2020"));
            Assert.That(identityDocument.VerificationMethod.ElementAt(1).Type, Is.EqualTo("JsonWebKey2020"));
            Assert.That(identityDocument.VerificationMethod.ElementAt(2).Type, Is.EqualTo("JsonWebKey2020"));
            Assert.That(identityDocument.VerificationMethod.ElementAt(3).Type, Is.EqualTo("JsonWebKey2020"));
        }

        [Test]
        public void When_Build_IdentityDocument_With_X25519KeyAgreementVerificationMethod_Then_Document_Is_Correct()
        {
            // ARRANGE
            var firstX25519 = X25519AgreementKey.Generate();
            var secondX25519 = X25519AgreementKey.Generate();
            var identityDocument = DidDocumentBuilder.New("did")
                .AddAlsoKnownAs("didSubject")
                .AddController("didController")
                .AddKeyAggreement(X25519KeyAgreementKey2019Standard.TYPE, firstX25519, "controller")
                .AddKeyAggreement(X25519KeyAgreementKey2020Standard.TYPE, secondX25519, "controller")
                .Build();

            // ACT
            var json = identityDocument.Serialize();
            var contextLst = JsonObject.Parse(json)["@context"] as JsonArray;

            // ASSERT
            Assert.That(identityDocument.Id, Is.EqualTo("did"));
            Assert.That(contextLst.ElementAt(0).ToString(), Is.EqualTo("https://www.w3.org/ns/did/v1"));
            Assert.That(contextLst.ElementAt(1).ToString(), Is.EqualTo("https://w3id.org/security/suites/x25519-2019/v1"));
            Assert.That(contextLst.ElementAt(2).ToString(), Is.EqualTo("https://w3id.org/security/suites/x25519-2020/v1"));
            Assert.That(identityDocument.AlsoKnownAs.First(), Is.EqualTo("didSubject"));
            Assert.That(identityDocument.Controller.ToString(), Is.EqualTo("didController"));
            Assert.That(identityDocument.VerificationMethod.ElementAt(0).Type, Is.EqualTo("X25519KeyAgreementKey2019"));
            Assert.That(identityDocument.VerificationMethod.ElementAt(1).Type, Is.EqualTo("X25519KeyAgreementKey2020"));
        }

        [Test]
        public void When_Build_IdentityDocument_With_EcdsaSecp256k1RecoveryMethod2020VerificationMethod_Then_Document_Is_Correct()
        {
            // ARRANGE
            var es256K = ES256KSignatureKey.Generate();
            var identityDocument = DidDocumentBuilder.New("did")
                .AddAlsoKnownAs("didSubject")
                .AddController("didController")
                .AddVerificationMethod(EcdsaSecp256k1RecoveryMethod2020Standard.TYPE, es256K, "controller", VerificationMethodUsages.AUTHENTICATION)
                .Build();

            // ACT
            var json = identityDocument.Serialize();
            var context = JsonObject.Parse(json)["@context"].ToString();

            // ASSERT
            Assert.That(identityDocument.Id, Is.EqualTo("did"));
            Assert.That(context, Is.EqualTo("[\r\n  \"https://www.w3.org/ns/did/v1\",\r\n  \"https://w3id.org/security/suites/secp256k1recovery-2020/v2\"\r\n]"));
            Assert.That(identityDocument.AlsoKnownAs.First(), Is.EqualTo("didSubject"));
            Assert.That(identityDocument.Controller.ToString(), Is.EqualTo("didController"));
            Assert.That(identityDocument.VerificationMethod.ElementAt(0).Type, Is.EqualTo("EcdsaSecp256k1RecoveryMethod2020"));
        }

        [Test]
        public void When_Build_IdentityDocument_With_EcdsaSecp256k1VerificationKey2019VerificationMethod_Then_Document_Is_Correct()
        {
            // ARRANGE
            var es256K = ES256KSignatureKey.Generate();
            var identityDocument = DidDocumentBuilder.New("did")
                .AddAlsoKnownAs("didSubject")
                .AddController("didController")
                .AddVerificationMethod(EcdsaSecp256k1VerificationKey2019Standard.TYPE, es256K, "controller", VerificationMethodUsages.AUTHENTICATION)
                .Build();

            // ACT
            var json = identityDocument.Serialize();
            var contextLst = JsonObject.Parse(json)["@context"] as JsonArray;

            // ASSERT
            Assert.That(identityDocument.Id, Is.EqualTo("did"));
            Assert.That(contextLst.ElementAt(0).ToString(), Is.EqualTo("https://www.w3.org/ns/did/v1"));
            Assert.That(contextLst.ElementAt(1).ToString(), Is.EqualTo("https://w3id.org/security/suites/secp256k1-2019/v1"));
            Assert.That(identityDocument.AlsoKnownAs.First(), Is.EqualTo("didSubject"));
            Assert.That(identityDocument.Controller.ToString(), Is.EqualTo("didController"));
            Assert.That(identityDocument.VerificationMethod.ElementAt(0).Type, Is.EqualTo("EcdsaSecp256k1VerificationKey2019"));
        }
    }
}
