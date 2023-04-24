// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using SimpleIdServer.Did;
using SimpleIdServer.Did.Extensions;

namespace SimpleIdServer.DID.Tests
{
    public class IdentityDocumentBuilderFixture
    {
        [Test]
        public void When_Build_IdentityDocument_Then_JSONIsCorrect()
        {
            var gen = new Ed25519KeyPairGenerator();
            var param = new Ed25519KeyGenerationParameters(new Org.BouncyCastle.Security.SecureRandom());
            gen.Init(param);
            var keyPair = gen.GenerateKeyPair();
            Ed25519PublicKeyParameters publicKeyParam = (Ed25519PublicKeyParameters)keyPair.Public;
            var hex = publicKeyParam.GetEncoded().ToHex();
            // tmp: c4c323b4ba114591579d92591b26e92e59aa5529c6adbebb820da7ca407e9d34
            // tmp: baf6a5e13f1bd4510148b2516e5d1b478ad1d3c742f7973edc804d10537fa3b6
            // List of supported keys : https://w3c-ccg.github.io/ld-cryptosuite-registry/
            // Generate this key : Ed25519VerificationKey2018
            var identityDocument = IdentityDocumentBuilder.New("did", "publicadr")
                .AddVerificationMethod(SignatureKeyBuilder.NewES256K(), VerificationMethodTypes.EcdsaSecp256k1VerificationKey2019)
                .Build();
            var json = identityDocument.Serialize();

            // tmp : 04ebafc30f377af345bb86c9269ed6432d6245b44f01dd410f8c0e73ab1801211c84b76fade77b4d6e27da82d051e3603b35c21072201e1a1c00073ab09d004ee4
            // tmp : 0460c7ec182bcc527ba718caba7dd797a3da81dc3a94d4d2cba0737288dc4f921e0e9196016b2d13913a8370ddd98f2e616b2eaf208f9c9ee22b7aaf0c1d869dad
            string ss = "";
        }
    }
}
