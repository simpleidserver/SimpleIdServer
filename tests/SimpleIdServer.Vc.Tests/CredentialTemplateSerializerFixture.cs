// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using SimpleIdServer.Vc.Builders;
using SimpleIdServer.Vc.Models;

namespace SimpleIdServer.Vc.Tests
{
    public class CredentialTemplateSerializerFixture
    {
        [Test]
        public void When_BuildAndSerializeW3CCredentialTemplate_Then_JsonIsCorrect()
        {
            var credentialTemplate = W3CCredentialTemplateBuilder.New("University Credential", "https://exampleuniversity.com/public/logo.png", "UniversityDegreeCredential")
                .AddStringCredentialSubject("given_name", false, new List<W3CCredentialSubjectDisplay>
                {
                    new W3CCredentialSubjectDisplay { Locale = "en-US", Name = "Given Name" }
                }).Build();
            var json = credentialTemplate.Serialize();
            Assert.NotNull(json);
        }
    }
}
