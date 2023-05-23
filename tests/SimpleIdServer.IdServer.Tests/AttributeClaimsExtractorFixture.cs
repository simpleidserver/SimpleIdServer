// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Extractors;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Tests
{
    public class AttributeClaimsExtractorFixture
    {
        [Test]
        public void When_Extract_Claims_From_Attributes_Then_JSONIsCorrect()
        {
            var lst = new List<CustomMappingRule>
            {
                new CustomMappingRule { SourceUserAttribute = "street", TargetClaimPath = "adr.street" },
                new CustomMappingRule { SourceUserAttribute = "postalCode", TargetClaimPath = "adr.postalCode" }
            };
            var user = new User
            {
                OAuthUserClaims = new List<UserClaim>
                {
                    new UserClaim { Name = "street", Value = "street NY" },
                    new UserClaim { Name = "postalCode", Value = "1200" }
                }
            };
            var extractor = new ClaimsExtractor(new IClaimExtractor[]
            {
                new AttributeClaimExtractor()
            });
            var dic = extractor.Extract(user, lst);
            var json = JsonSerializer.Serialize(dic);
            string ss = "";
        }

        public class CustomMappingRule : IClaimMappingRule
        {
            public MappingRuleTypes MapperType { get; set; }
            public string? SourceUserAttribute { get; set; }
            public string? SourceUserProperty { get; set; }
            public string TargetClaimPath { get; set; } = null!;
            public TokenClaimJsonTypes? TokenClaimJsonType { get; set; }
        }
    }
}
