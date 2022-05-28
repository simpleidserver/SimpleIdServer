// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Domains.Builders;
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
               .AddStringAttribute("firstName")
               .AddStringAttribute("lastName")
               .AddStringAttribute("roles", multiValued: true)
               .AddStringAttribute("familyName")
               .AddComplexAttribute("scimRoles", opt =>
               {
                   opt.AddStringAttribute("value");
                   opt.AddStringAttribute("type");
                   opt.AddStringAttribute("display");
               }, multiValued: true, mutability: SCIMSchemaAttributeMutabilities.READWRITE)
               .AddComplexAttribute("adRoles", opt =>
               {
                   opt.AddStringAttribute("display");
                   opt.AddStringAttribute("value");
               }, multiValued: true)
               .AddComplexAttribute("attributes", opt =>
               {
                   opt.AddStringAttribute("title");
                   opt.AddComplexAttribute("subattributes", sopt =>
                   {
                       sopt.AddStringAttribute("str");
                       sopt.AddStringAttribute("sstr");
                   }, multiValued: true);
                   opt.AddComplexAttribute("subtitle", sopt =>
                   {
                       sopt.AddStringAttribute("str");
                   });
               }, multiValued: true)
               .AddComplexAttribute("phones", opt =>
               {
                   opt.AddStringAttribute("phoneNumber", description: "Phone number");
                   opt.AddStringAttribute("type", description: "Type");
               }, multiValued: true, mutability: SCIMSchemaAttributeMutabilities.READWRITE)
               .Build();
            var userRepresentation = SCIMRepresentationBuilder.Create(new List<SCIMSchema> { userSchema })
                .AddStringAttribute("userName", "urn:ietf:params:scim:schemas:core:2.0:User", new List<string> { "john" })
                .AddStringAttribute("lastName", "urn:ietf:params:scim:schemas:core:2.0:User", new List<string> { "lastName" })
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
                .AddComplexAttribute("scimRoles", "urn:ietf:params:scim:schemas:core:2.0:User", (b) =>
                {
                    b.AddStringAttribute("value", new List<string> { "firstRole" });
                    b.AddStringAttribute("type", new List<string> { "roleType" });
                })
                .AddComplexAttribute("scimRoles", "urn:ietf:params:scim:schemas:core:2.0:User", (b) =>
                {
                    b.AddStringAttribute("value", new List<string> { "secondRole" });
                })
                .AddComplexAttribute("attributes", "urn:ietf:params:scim:schemas:core:2.0:User", (b) =>
                {
                    b.AddStringAttribute("title", new List<string> { "title" });
                    b.AddComplexAttribute("subattributes", (sb) =>
                    {
                        sb.AddStringAttribute("str", new List<string> { "1" });
                    });
                    b.AddComplexAttribute("subtitle", (sb) =>
                    {
                        sb.AddStringAttribute("str", new List<string> { "1" });
                    });
                })
                .AddComplexAttribute("attributes", "urn:ietf:params:scim:schemas:core:2.0:User", (b) =>
                {
                    b.AddStringAttribute("title", new List<string> { "secondTitle" });
                    b.AddComplexAttribute("subattributes", (sb) =>
                    {
                        sb.AddStringAttribute("str", new List<string> { "1" });
                    });
                })
                .AddComplexAttribute("adRoles", "urn:ietf:params:scim:schemas:core:2.0:User", (b) =>
                {
                    b.AddStringAttribute("display", new List<string> { "display" });
                    b.AddStringAttribute("value", new List<string> { "user1" });
                })
                .AddComplexAttribute("adRoles", "urn:ietf:params:scim:schemas:core:2.0:User", (b) =>
                {
                    b.AddStringAttribute("value", new List<string> { "user2" });
                })
                .AddComplexAttribute("adRoles", "urn:ietf:params:scim:schemas:core:2.0:User", (b) =>
                {
                    b.AddStringAttribute("value", new List<string> { "user3" });
                })
                .Build();

            userRepresentation.ApplyPatches(new List<PatchOperationParameter>
            {
                new PatchOperationParameter
                {
                    Operation = SCIMPatchOperations.REPLACE,
                    Path = "phones.phoneNumber",
                    Value = "NEWPHONE"
                },
                new PatchOperationParameter
                {
                    Operation = SCIMPatchOperations.REPLACE,
                    Path = "userName",
                    Value = "cassandra"
                },
                new PatchOperationParameter
                {
                    Operation = SCIMPatchOperations.ADD,
                    Path = "phones",
                    Value = JArray.Parse("[{ phoneNumber : '03', type: 'type1' }, { phoneNumber : '05', type: 'type2' }]")
                },
                new PatchOperationParameter
                {
                    Operation = SCIMPatchOperations.REPLACE,
                    Path = "phones[phoneNumber eq 03].type",
                    Value = "newType"
                },
                new PatchOperationParameter
                {
                    Operation = SCIMPatchOperations.REMOVE,
                    Path = "phones[phoneNumber eq 01]"
                },
                new PatchOperationParameter
                {
                    Operation = SCIMPatchOperations.REMOVE,
                    Path = "phones[phoneNumber eq 03].phoneNumber"
                },
                new PatchOperationParameter
                {
                    Operation = SCIMPatchOperations.ADD,
                    Path = "familyName",
                    Value = "familyName"
                },
                new PatchOperationParameter
                {
                    Operation = SCIMPatchOperations.ADD,
                    Path = "familyName",
                    Value = "updatedFamilyName"
                },
                new PatchOperationParameter
                {
                    Operation = SCIMPatchOperations.ADD,
                    Path = "roles",
                    Value = "firstRole"
                },
                new PatchOperationParameter
                {
                    Operation = SCIMPatchOperations.ADD,
                    Path = "roles",
                    Value = "secondRole"
                },
                new PatchOperationParameter
                {
                    Operation = SCIMPatchOperations.REPLACE,
                    Value = JObject.Parse("{ 'firstName' : 'firstName' }")
                },
                new PatchOperationParameter
                {
                    Operation = SCIMPatchOperations.REPLACE,
                    Value = JObject.Parse("{ 'lastName' : 'updatedLastName' }")
                },
                new PatchOperationParameter
                {
                    Operation = SCIMPatchOperations.ADD,
                    Value = JObject.Parse("{ 'phoneNumber': '06', 'type': 'mobile' }"),
                    Path = "phones"
                },
                new PatchOperationParameter
                {
                    Operation = SCIMPatchOperations.ADD,
                    Path = "attributes[title eq title].subattributes",
                    Value = JObject.Parse("{ 'str' : '2' }")
                },
                new PatchOperationParameter
                {
                    Operation = SCIMPatchOperations.ADD,
                    Path = "attributes[title eq title].subtitle",
                    Value = JObject.Parse("{ 'str' : '2' }")
                },
                new PatchOperationParameter
                {
                    Operation = SCIMPatchOperations.REPLACE,
                    Path = "attributes[title eq secondTitle].subattributes",
                    Value = JObject.Parse("{ 'str' : '3' }")
                },
                new PatchOperationParameter
                {
                    Operation = SCIMPatchOperations.REPLACE,
                    Path = "adRoles.display",
                    Value = "NEWUSER"
                },
                new PatchOperationParameter
                {
                    Operation = SCIMPatchOperations.ADD,
                    Path = "adRoles[value eq user3].display",
                    Value = "NEWUSER3"
                },
                new PatchOperationParameter
                {
                    Operation = SCIMPatchOperations.REPLACE,
                    Path = "scimRoles",
                    Value = JArray.Parse("[{ 'value': 'firstRole', 'type' : 'newType' }]")
                }
            }, new List<SCIMAttributeMapping>
            {
                new SCIMAttributeMapping
                {
                    SourceAttributeId = userSchema.Attributes.First(a => a.Name == "scimRoles").Id
                }
            }, false);

            Assert.True(userRepresentation.FlatAttributes.Count(a => a.SchemaAttribute.FullPath == "adRoles.display" && a.ValueString == "NEWUSER") == 2);
            Assert.True(userRepresentation.FlatAttributes.Any(a => a.SchemaAttribute.FullPath == "adRoles.value" && a.ValueString == "user1") == true);
            Assert.True(userRepresentation.FlatAttributes.Any(a => a.SchemaAttribute.FullPath == "adRoles.value" && a.ValueString == "user2") == true);
            Assert.True(userRepresentation.FlatAttributes.Any(a => a.SchemaAttribute.FullPath == "adRoles.value" && a.ValueString == "user3") == true);
            Assert.True(userRepresentation.FlatAttributes.Any(a => a.SchemaAttribute.FullPath == "adRoles.display" && a.ValueString == "NEWUSER3") == true);
            Assert.Equal("cassandra", userRepresentation.FlatAttributes.First(a => a.SchemaAttribute.Name == "userName").ValueString);
            Assert.True(userRepresentation.FlatAttributes.Any(a => a.SchemaAttribute.Name == "phoneNumber" && a.ValueString == "03") == false);
            Assert.True(userRepresentation.FlatAttributes.Any(a => a.SchemaAttribute.Name == "phoneNumber" && a.ValueString == "05") == true);
            Assert.True(userRepresentation.FlatAttributes.Any(a => a.SchemaAttribute.Name == "phoneNumber" && a.ValueString == "01") == false);
            Assert.True(userRepresentation.FlatAttributes.Any(a => a.SchemaAttribute.Name == "phoneNumber" && a.ValueString == "06") == true);
            Assert.True(userRepresentation.FlatAttributes.Any(a => a.SchemaAttribute.Name == "familyName" && a.ValueString == "updatedFamilyName") == true);
            Assert.True(userRepresentation.FlatAttributes.Any(a => a.SchemaAttribute.Name == "roles" && a.ValueString == "firstRole") == true);
            Assert.True(userRepresentation.FlatAttributes.Any(a => a.SchemaAttribute.Name == "roles" && a.ValueString == "secondRole") == true);
            Assert.True(userRepresentation.FlatAttributes.Any(a => a.SchemaAttribute.Name == "userName" && a.ValueString == "cassandra") == true);
            Assert.True(userRepresentation.FlatAttributes.Any(a => a.SchemaAttribute.Name == "lastName" && a.ValueString == "updatedLastName") == true);
            Assert.True(userRepresentation.FlatAttributes.Any(a => a.SchemaAttribute.FullPath == "attributes.subattributes.str" && a.ValueString == "2") == true);
            Assert.True(userRepresentation.FlatAttributes.Any(a => a.SchemaAttribute.FullPath == "attributes.subtitle.str" && a.ValueString == "2") == true);
            Assert.True(userRepresentation.FlatAttributes.Any(a => a.SchemaAttribute.FullPath == "attributes.subattributes.str" && a.ValueString == "3") == true);
            Assert.True(userRepresentation.FlatAttributes.Any(a => a.SchemaAttribute.FullPath == "scimRoles.value" && a.ValueString == "firstRole") == true);
            Assert.True(userRepresentation.FlatAttributes.Any(a => a.SchemaAttribute.FullPath == "scimRoles.value" && a.ValueString == "secondRole") == false);
        }
    }
}
