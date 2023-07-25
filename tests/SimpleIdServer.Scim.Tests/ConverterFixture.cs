// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json;
using SimpleIdServer.Scim.DTOs;
using Xunit;

namespace SimpleIdServer.Scim.Tests
{
    public class ConverterFixture
    {
        [Fact]
        public void When_Serialize_RepresentationParameter_Then_JsonIsReturned()
        {
            var parameter = new RepresentationParameter { ExternalId = "externalid" };
            var json = JsonConvert.SerializeObject(parameter);
            Assert.NotNull(json);
        }

        [Fact]
        public void When_Serialize_PatchOperationParameter_Then_JsonIsReturned()
        {
            var parameter = new PatchOperationParameter { Path = "path" };
            var json = JsonConvert.SerializeObject(parameter);
            Assert.NotNull(json);
        }
    }
}
