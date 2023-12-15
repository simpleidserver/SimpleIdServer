// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using SimpleIdServer.Did.Crypto;

namespace SimpleIdServer.DID.Tests
{
    public class ES256SignatureKeyFixture
    {
        [Test]
        public void When_BuildSignature_Then_SignatureIsCorrect()
        {
            var privateKey = "040f1dbf0a2ca86875447a7c010b0fc6d39d76859c458fbe8f2bf775a40ad74a";
            var plaintext = "thequickbrownfoxjumpedoverthelazyprogrammer";
            // var key = new ES256SignatureKey(null, privateKey.HexToByteArray());
            ES256SignatureKey key = null;
            var signature = key.Sign(plaintext);
            Assert.That(signature, Is.EqualTo("vOTe64WujVUjEiQrAlwaPJtNADx4usSlCfe8OXHS6Np1BqJdqdJX912pVwVlAjmbqR_TMVE5i5TWB_GJVgrHgg"));
        }
    }
}
