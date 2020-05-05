// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Builder;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.DTOs;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SimpleIdServer.Scim.Tests
{
    public class SCIMRepresentationPatchFixture
    {
        [Fact]
        public void When_Apply_Patch_To_SCIM_Representation()
        {
            var userSchema = SCIMSchemaBuilder.Create("urn:ietf:params:scim:schemas:core:2.0:User", "User", "User Account")
               .AddStringAttribute("userName", caseExact: true, uniqueness: SCIMSchemaAttributeUniqueness.SERVER)
               .AddComplexAttribute("phones", opt =>
               {
                   opt.AddStringAttribute("phoneNumber", description: "Phone number");
                   opt.AddStringAttribute("type", description: "Type");
               }, multiValued: true, mutability: SCIMSchemaAttributeMutabilities.READWRITE)
               .Build();
            var userRepresentation = SCIMRepresentationBuilder.Create(new List<SCIMSchema> { userSchema })
                .AddStringAttribute("userName", "urn:ietf:params:scim:schemas:core:2.0:User", new List<string> { "john" })
                .AddComplexAttribute("phones", "urn:ietf:params:scim:schemas:core:2.0:User", (b) =>
                {
                    b.AddStringAttribute("phoneNumber", new List<string> { "01" });
                    b.AddStringAttribute("type", new List<string> { "mobile" });
                })
                .AddComplexAttribute("phones", "urn:ietf:params:scim:schemas:core:2.0:User", (b) =>
                {
                    b.AddStringAttribute("phoneNumber", new List<string> { "02" });
                    b.AddStringAttribute("type", new List<string> { "home" });
                })
                .Build();

            userRepresentation.ApplyPatches(new List<SCIMPatchOperationRequest>
            {
                new SCIMPatchOperationRequest(SCIMPatchOperations.REPLACE, "userName", "cassandra"),
                new SCIMPatchOperationRequest(SCIMPatchOperations.ADD, "phones", JArray.Parse("[{ phoneNumber : '03', type: 'type1' }, { phoneNumber : '05', type: 'type2' }]")),
                new SCIMPatchOperationRequest(SCIMPatchOperations.REMOVE, "phones[phoneNumber eq 01]")
            }, false);

            Assert.Equal("cassandra", userRepresentation.Attributes.First(a => a.SchemaAttribute.Name == "userName").ValuesString.First());
            Assert.True(userRepresentation.Attributes.Any(a => a.SchemaAttribute.Name == "phones" && a.Values.Any(b => b.SchemaAttribute.Name == "phoneNumber" && b.ValuesString.Contains("03"))) == true);
            Assert.True(userRepresentation.Attributes.Any(a => a.SchemaAttribute.Name == "phones" && a.Values.Any(b => b.SchemaAttribute.Name == "phoneNumber" && b.ValuesString.Contains("05"))) == true);
            Assert.True(userRepresentation.Attributes.Any(a => a.SchemaAttribute.Name == "phones" && a.Values.Any(b => b.SchemaAttribute.Name == "phoneNumber" && b.ValuesString.Contains("01"))) == false);
        }
    }
}
