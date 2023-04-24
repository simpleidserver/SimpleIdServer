// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;
using SimpleIdServer.Did.Crypto;

namespace SimpleIdServer.DID.Tests
{
    public class Ed25519SignatureKeyFixture
    {
        [Test]
        public void When_BuildSignature_Then_SignatureIsCorrect()
        {
            var privateKey = "nlXR4aofRVuLqtn9+XVQNlX4s1nVQvp+TOhBBtYls1IG+sHyIkDP/WN+rWZHGIQp+v2pyct+rkM4asF/YRFQdQ==";
            var plaintext = "hello";
            var key = new Ed25519SignatureKey(null, Base64UrlEncoder.DecodeBytes(privateKey));
            var signature = key.Sign(plaintext);
            Assert.That(signature, Is.EqualTo("lLY_SeplJc_4tgMP1BHmjfxS0UEi-Xvonzbss4GT7yuFz--H28uCwsRjlIwXL4I0ugCrM-zQoA2gW2JdnFRkDQ"));
        }

        [Test]
        public void When_CheckSignature_Then_SignatureIsCorrect()
        {
            var privateKey = "nlXR4aofRVuLqtn9+XVQNlX4s1nVQvp+TOhBBtYls1IG+sHyIkDP/WN+rWZHGIQp+v2pyct+rkM4asF/YRFQdQ==";
            var signature = "lLY_SeplJc_4tgMP1BHmjfxS0UEi-Xvonzbss4GT7yuFz--H28uCwsRjlIwXL4I0ugCrM-zQoA2gW2JdnFRkDQ";
            var plaintext = "hello";
            var key = new Ed25519SignatureKey(null, Base64UrlEncoder.DecodeBytes(privateKey));
            var isSignatureValid = key.Check(plaintext, signature);
            Assert.True(isSignatureValid);
        }
    }
}
