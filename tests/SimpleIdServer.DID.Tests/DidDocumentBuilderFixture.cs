// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using SimpleIdServer.Did;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Models;

namespace SimpleIdServer.DID.Tests
{
    public class DidDocumentBuilderFixture
    {
        [Test]
        public void When_Build_IdentityDocument_Then_JSONIsCorrect()
        {
            var identityDocument = DidDocumentBuilder.New("did")
                .AddContext("https://w3id.org/security/suites/ed25519-2020/v1")
                .AddContext(c =>
                {
                    c.AddPropertyValue("foo", "https://example.com/property");
                    c.AddPropertyId("image", "http://schema.org/image");
                })
                .AddAlsoKnownAs("didSubject")
                .AddController("didController")
                // .AddJsonWebKeyVerificationMethod(Ed25519SignatureKey.New(), "controller")
                .AddJsonWebKeyVerificationMethod(ES256KSignatureKey.Generate(), "controller", DidDocumentVerificationMethodUsages.AUTHENTICATION)
                .AddJsonWebKeyVerificationMethod(ES256SignatureKey.Generate(), "controller", DidDocumentVerificationMethodUsages.AUTHENTICATION)
                .AddJsonWebKeyVerificationMethod(ES384SignatureKey.Generate(), "controller", DidDocumentVerificationMethodUsages.AUTHENTICATION)
                // .AddJsonWebKeyVerificationMethod(RSA2048SignatureKey.New(), "controller", DidDocumentVerificationMethodUsages.AUTHENTICATION)
                .AddPublicKeyMultibaseVerificationMethod(ES256KSignatureKey.Generate(), "controller", DidDocumentVerificationMethodUsages.AUTHENTICATION)
                .AddJsonWebKeyAssertionMethod(ES256KSignatureKey.Generate(), "controller")
                .Build();


            // Authentication.
            // Entity associated with the value of the controller, need to authenticate with its own DID Document and associated authentication.
            // Example : Parent creates and maintains control of a DID for a child
            // A corporation creates and maintains control of a DID for a subsidiary
            // Manfuacturer creates and maintans control of a DID for a product.

            var json = identityDocument.Serialize();
            Assert.NotNull(json);
        }

        [Test]
        public void When_Deserialize_IdentityDocument()
        {
            // 1. Deserialize identity document.
            // 2. Resolve identity document : pour le KEY : OK
            // 3. Check proof : https://openid.github.io/OpenID4VCI/openid-4-verifiable-credential-issuance-wg-draft.html#name-verifying-key-proof
        }

        [Test]
        public void When_Resolve_IdentityDocument()
        {

        }

        [Test]
        public void When_Manage_Update_Controller()
        {
            // https://nuts-node.readthedocs.io/en/latest/pages/technology/did.html#controller
            // https://nuts-foundation.gitbook.io/v1/rfc/rfc006-distributed-registry
            // Changes to DID documents are only accepted when the network transaction is signed with a controller's authentication key.
            // Il faut créer deux DID différents !!!!

            // https://github.com/uport-project/ethr-did-registry/blob/master/contracts/EthereumDIDRegistry.sol
            // Un seul owner pour le smart contract !
            // https://etherscan.io/address/0xdca7ef03e98e0dc2b855be647c39abe984fcf21b#code

            // https://learn.mattr.global/tutorials/dids/did-ion
            // Vérifier si il existe plusieurs controller.
        }
    }
}
