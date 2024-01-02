// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using SimpleIdServer.Did;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Formatters;
using SimpleIdServer.Did.Models;
using System.Text.Json.Nodes;

namespace SimpleIdServer.DID.Tests
{
    public class DidDocumentBuilderFixture
    {
        [Test]
        public void When_Build_IdentityDocument_With_Ed25519VerificationKey2020VerificationMethods_Then_DocumentIsCorrect()
        {
            // https://www.w3.org/TR/did-spec-registries/#verification-method-types
            // ARRANGE
            var ed25119Sig = Ed25519SignatureKey.Generate();
            var es256K = ES256KSignatureKey.Generate();
            var es256 = ES256SignatureKey.Generate();
            var es384 = ES384SignatureKey.Generate();
            var x25519 = X25519AgreementKey.Generate();
            var publicKeyMultibaseFormatter = FormatterFactory.BuildEd25519VerificationKey2020Formatter();
            var identityDocument = DidDocumentBuilder.New("did")
                .AddAlsoKnownAs("didSubject")
                .AddController("didController")
                .AddEd25519VerificationKey2020VerificationMethod(ed25119Sig, "controller", VerificationMethodUsages.AUTHENTICATION)
                .AddEd25519VerificationKey2020VerificationMethod(es256K, "controller", VerificationMethodUsages.AUTHENTICATION)
                .AddEd25519VerificationKey2020VerificationMethod(es256, "controller", VerificationMethodUsages.AUTHENTICATION)
                .AddEd25519VerificationKey2020VerificationMethod(es384, "controller", VerificationMethodUsages.AUTHENTICATION)
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
            Assert.That(identityDocument.VerificationMethod.ElementAt(1).Type, Is.EqualTo("Ed25519VerificationKey2020"));
            Assert.That(identityDocument.VerificationMethod.ElementAt(2).Type, Is.EqualTo("Ed25519VerificationKey2020"));
            Assert.That(identityDocument.VerificationMethod.ElementAt(3).Type, Is.EqualTo("Ed25519VerificationKey2020"));
            Assert.That(identityDocument.VerificationMethod.ElementAt(4).Type, Is.EqualTo("Ed25519VerificationKey2020"));
            Assert.True(publicKeyMultibaseFormatter.Extract(identityDocument.VerificationMethod.ElementAt(0)).GetPublicKey().SequenceEqual(ed25119Sig.GetPublicKey()));
            Assert.True(publicKeyMultibaseFormatter.Extract(identityDocument.VerificationMethod.ElementAt(1)).GetPublicKey().SequenceEqual(es256K.GetPublicKey()));
            Assert.True(publicKeyMultibaseFormatter.Extract(identityDocument.VerificationMethod.ElementAt(2)).GetPublicKey().SequenceEqual(es256.GetPublicKey()));
            Assert.True(publicKeyMultibaseFormatter.Extract(identityDocument.VerificationMethod.ElementAt(3)).GetPublicKey().SequenceEqual(es384.GetPublicKey()));
            Assert.True(publicKeyMultibaseFormatter.Extract(identityDocument.VerificationMethod.ElementAt(4)).GetPublicKey().SequenceEqual(x25519.GetPublicKey()));
        }

        [Test]
        public void When_Build_IdentityDocument_With_JwkVerificationMethods_Then_DocumentIsCorrect()
        {
            // ARRANGE
            var ed25119Sig = Ed25519SignatureKey.Generate();
            var es256K = ES256KSignatureKey.Generate();
            var es256 = ES256SignatureKey.Generate();
            var es384 = ES384SignatureKey.Generate();
            var jwkFormatter = FormatterFactory.BuildJsonWebKey2020Formatter();
            var identityDocument = DidDocumentBuilder.New("did")
                .AddAlsoKnownAs("didSubject")
                .AddController("didController")
                .AddJsonWebKeyVerificationMethod(ed25119Sig, "controller", VerificationMethodUsages.AUTHENTICATION)
                .AddJsonWebKeyVerificationMethod(es256K, "controller", VerificationMethodUsages.AUTHENTICATION)
                .AddJsonWebKeyVerificationMethod(es256, "controller", VerificationMethodUsages.AUTHENTICATION)
                .AddJsonWebKeyVerificationMethod(es384, "controller", VerificationMethodUsages.AUTHENTICATION)
                .Build();

            // ACT
            var json = identityDocument.Serialize();
            var contextLst = JsonObject.Parse(json)["@context"] as JsonArray;

            Assert.IsNotNull(json);
            Assert.That(identityDocument.Id, Is.EqualTo("did"));
            Assert.That(contextLst.ElementAt(0).ToString(), Is.EqualTo("https://www.w3.org/ns/did/v1"));
            Assert.That(contextLst.ElementAt(1).ToString(), Is.EqualTo("https://w3id.org/security/suites/jws-2020/v1"));
            Assert.That(identityDocument.AlsoKnownAs.First(), Is.EqualTo("didSubject"));
            Assert.That(identityDocument.Controller.ToString(), Is.EqualTo("didController"));
            Assert.That(identityDocument.VerificationMethod.ElementAt(0).Type, Is.EqualTo("JsonWebKey2020"));
            Assert.That(identityDocument.VerificationMethod.ElementAt(1).Type, Is.EqualTo("JsonWebKey2020"));
            Assert.That(identityDocument.VerificationMethod.ElementAt(2).Type, Is.EqualTo("JsonWebKey2020"));
            Assert.That(identityDocument.VerificationMethod.ElementAt(3).Type, Is.EqualTo("JsonWebKey2020"));
            Assert.That(identityDocument.VerificationMethod.ElementAt(4).Type, Is.EqualTo("JsonWebKey2020"));
            Assert.True(jwkFormatter.Extract(identityDocument.VerificationMethod.ElementAt(0)).GetPublicKey().SequenceEqual(ed25119Sig.GetPublicKey()));
            Assert.True(jwkFormatter.Extract(identityDocument.VerificationMethod.ElementAt(1)).GetPublicKey().SequenceEqual(es256K.GetPublicKey()));
            Assert.True(jwkFormatter.Extract(identityDocument.VerificationMethod.ElementAt(2)).GetPublicKey().SequenceEqual(es256.GetPublicKey()));
            Assert.True(jwkFormatter.Extract(identityDocument.VerificationMethod.ElementAt(3)).GetPublicKey().SequenceEqual(es384.GetPublicKey()));
        }

        [Test]
        public void When_BuildIdentityDocument_With_X25519KeyAgreementVerificationMethods_Then_Document_Is_Correct()
        {
            // ARRANGE
            var x25519 = X25519AgreementKey.Generate();
            var keyAgreementFormatter = FormatterFactory.BuildX25519KeyAgreementFormatter();
            var identityDocument = DidDocumentBuilder.New("did")
                .AddAlsoKnownAs("didSubject")
                .AddController("didController")
                .AddX25519KeyAgreementVerificationMethod(x25519, "controller")
                .Build();

            // ACT
            var json = identityDocument.Serialize();
            var contextLst = JsonObject.Parse(json)["@context"] as JsonArray;

            Assert.IsNotNull(json);
            Assert.That(identityDocument.Id, Is.EqualTo("did"));
            Assert.That(contextLst.ElementAt(0).ToString(), Is.EqualTo("https://www.w3.org/ns/did/v1"));
            Assert.That(contextLst.ElementAt(1).ToString(), Is.EqualTo("https://w3id.org/security/suites/x25519-2019/v1"));
            Assert.That(identityDocument.AlsoKnownAs.First(), Is.EqualTo("didSubject"));
            Assert.That(identityDocument.Controller.ToString(), Is.EqualTo("didController"));
            Assert.That(identityDocument.VerificationMethod.ElementAt(0).Type, Is.EqualTo("X25519KeyAgreementKey2019"));
            Assert.True(keyAgreementFormatter.Extract(identityDocument.VerificationMethod.ElementAt(0)).GetPublicKey().SequenceEqual(x25519.GetPublicKey()));
        }

        [Test]
        public void When_BuildIdentityDocument_And_ExtractPrivateKey_In_MulticodecFormat_Then_PrivateKeyExists()
        {
            // ARRANGE
            var ed25119Sig = Ed25519SignatureKey.Generate();
            var identityDocument = DidDocumentBuilder.New("did", true)
                .AddAlsoKnownAs("didSubject")
                .AddController("didController")
                .AddEd25519VerificationKey2020VerificationMethod(ed25119Sig, "controller", VerificationMethodUsages.AUTHENTICATION)
                .Build();

            // ACT
            var json = identityDocument.Serialize();

            // ASSERT
            Assert.That(identityDocument.VerificationMethod.ElementAt(0).Type, Is.EqualTo("Ed25519VerificationKey2020"));
            Assert.IsNotNull(identityDocument.VerificationMethod.ElementAt(0).SecretKeyMultibase);
        }
    }
}
