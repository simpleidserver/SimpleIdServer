// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using SimpleIdServer.Did;

namespace SimpleIdServer.DID.Tests
{
    public class IdentityDocumentBuilderFixture
    {
        [Test]
        public void When_Build_IdentityDocument_Then_JSONIsCorrect()
        {
            var identityDocument = IdentityDocumentBuilder.New("did", "publicadr")
                .SetContext()
                .AddVerificationMethod(SignatureKeyBuilder.NewES256K(), SimpleIdServer.Did.Constants.VerificationMethodTypes.EcdsaSecp256k1VerificationKey2019)
                .Build();
            var json = identityDocument.Serialize();
            Assert.NotNull(json);
        }
    }
}
