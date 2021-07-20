// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Builders;
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Helpers;
using SimpleIdServer.Saml.Xsd;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace SimpleIdServer.Saml.Tests.Builders
{
    public class AuthnRequestBuilderFixture
    {
        [Fact]
        public void When_Build_And_Sign_AuthnRequest()
        {
            var payload = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "localhost.pfx"));
            var certificate = new X509Certificate2(payload, "password");
            // https://en.wikipedia.org/wiki/SAML_2.0
            // https://docs.oasis-open.org/security/saml/v2.0/saml-core-2.0-os.pdf
            // https://docs.oasis-open.org/security/saml/v2.0/saml-profiles-2.0-os.pdf
            // http://docs.oasis-open.org/security/saml/Post2.0/sstc-saml2-holder-of-key-cs-02.html
            // https://developers.onelogin.com/saml/examples/authnrequest : explains how to sign AuthnRequest
            // https://dtservices.bosa.be/sites/default/files/content/download/files/fas_saml_integration_guide_v0.51_1.pdf
            // ARRANGE
            var builder = AuthnRequestBuilder.New("SP")
                .SetIssuer(Constants.NameIdentifierFormats.EntityIdentifier, "urn:rp")
                .SetNameIDPolicy(Constants.NameIdentifierFormats.EmailAddress, null, true)
                .AddAuthnContextClassPassword(AuthnContextComparisonType.exact);

            // ACT 
            var authnRequest = builder.SignAndBuild(certificate, SignatureAlgorithms.RSASHA1, CanonicalizationMethods.C14);

            // ASSERT
            var xml = authnRequest.SerializeXml();
            var test = Compression.Compress("str");

            string sss = "";
        }
    }
}
