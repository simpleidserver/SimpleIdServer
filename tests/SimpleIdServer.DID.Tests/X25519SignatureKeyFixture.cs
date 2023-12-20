// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;

namespace SimpleIdServer.DID.Tests;

public class X25519SignatureKeyFixture
{
    [Test]
    public void Test()
    {
        // Algorithm used to encrypt a value.
        // https://github.com/dvsekhvalnov/jose-jwt/blob/cb426b83e1e2010cdb7b9c062532ce1b27905f71/jose-jwt/keys/EccKey.cs#L7

        // 
    }

    // https://github.com/novotnyllc/bc-csharp/blob/7b21218624e23ecb6c52395365140c12ec0370e4/crypto/test/src/crypto/test/X25519Test.cs#L14
    // https://github.com/samuel-lucas6/Geralt/blob/cd00f9d4b3539d24a5c4363bee8e07a116fc4abf/src/Geralt.Tests/X25519Tests.cs#L4
    // TODO !!!
    // https://github.com/digitalbazaar/x25519-key-agreement-key-2020/blob/main/test/X25519KeyAgreementKey2020.spec.js
    // https://zcloaknetwork.medium.com/learn-by-doing-did-protocol-and-verifiable-credential-32b4640cabdc

    // Un premier DID possède une clef X2559KeyAggrement
    // Un second DID possède une clef X2559KeyAggrement
    // Encrypter pour les deux
    // Decrypter les deux
}
