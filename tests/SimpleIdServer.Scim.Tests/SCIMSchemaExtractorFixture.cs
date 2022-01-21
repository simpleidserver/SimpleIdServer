// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using System.IO;
using Xunit;

namespace SimpleIdServer.Scim.Tests
{
    public class SCIMSchemaExtractorFixture
    {
        [Fact]
        public static void When_Extract_SCIMSchema()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Schemas", "UserSchema.json");
            var result = SCIMSchemaExtractor.Extract(filePath, SCIMEndpoints.User);
            Assert.NotNull(result);
        }
    }
}