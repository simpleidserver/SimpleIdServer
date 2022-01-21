// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Domains.Builders;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Parser;
using SimpleIdServer.Scim.Parser.Expressions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SimpleIdServer.Scim.Tests
{
    public class AttributeFilterFixture
    {
        [Fact]
        public void When_Extract_Attributes()
        {
            var userSchema = SCIMSchemaBuilder.Create("urn:ietf:params:scim:schemas:core:2.0:User", "User", "User Account")
               .AddStringAttribute("userName", caseExact: true, uniqueness: SCIMSchemaAttributeUniqueness.SERVER)
               .AddComplexAttribute("phones", opt =>
               {
                   opt.AddStringAttribute("phoneNumber", description: "Phone number");
                   opt.AddStringAttribute("type", description: "Type");
               }, multiValued: true, mutability: SCIMSchemaAttributeMutabilities.READWRITE)
               .AddComplexAttribute("info", opt =>
               {
                   opt.AddDecimalAttribute("age", description: "Age");
               }, multiValued: false, mutability: SCIMSchemaAttributeMutabilities.READWRITE)
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
                .AddComplexAttribute("info", "urn:ietf:params:scim:schemas:core:2.0:User", (b) =>
                {
                    b.AddDecimalAttribute("age", new List<decimal> { 20 });
                })
                .Build();

            var firstFilter = SCIMFilterParser.Parse("phones.phoneNumber", new List<SCIMSchema> { userSchema }) as SCIMAttributeExpression;
            var secondFilter = SCIMFilterParser.Parse("phones[phoneNumber eq 02]", new List<SCIMSchema> { userSchema }) as SCIMAttributeExpression;
            var thirdFilter = SCIMFilterParser.Parse("userName", new List<SCIMSchema> { userSchema }) as SCIMAttributeExpression;
            var fourthFilter = SCIMFilterParser.Parse("phones.phoneNumber", new List<SCIMSchema> { userSchema }) as SCIMAttributeExpression;
            var fifthFilter = SCIMFilterParser.Parse("phones[phoneNumber eq 02]", new List<SCIMSchema> { userSchema }) as SCIMAttributeExpression;
            var sixFilter = SCIMFilterParser.Parse("meta.lastModified", new List<SCIMSchema> { userSchema, StandardSchemas.StandardResponseSchemas }) as SCIMAttributeExpression;
            var sevenFilter = SCIMFilterParser.Parse("meta.lastModified", new List<SCIMSchema> { userSchema, StandardSchemas.StandardResponseSchemas }) as SCIMAttributeExpression;
            var eightFilter = SCIMFilterParser.Parse("id", new List<SCIMSchema> { userSchema }) as SCIMAttributeExpression;
            var nineFilter = SCIMFilterParser.Parse("id", new List<SCIMSchema> { userSchema }) as SCIMAttributeExpression;
            var tenFilter = SCIMFilterParser.Parse("info.age", new List<SCIMSchema> { userSchema }) as SCIMAttributeExpression;

            var firstJSON = IncludeAttributes(userRepresentation, new List<SCIMAttributeExpression> { firstFilter });
            var secondJSON = IncludeAttributes(userRepresentation, new List<SCIMAttributeExpression> { secondFilter }, isComplex: true);
            var thirdJSON = ExcludeAttributes(userRepresentation, new List<SCIMAttributeExpression> { thirdFilter }, "http://localhost");
            var fourthJSON = ExcludeAttributes(userRepresentation, new List<SCIMAttributeExpression> { fourthFilter }, "http://localhost");
            var fifthJSON = ExcludeAttributes(userRepresentation, new List<SCIMAttributeExpression> { fifthFilter }, "http://localhost");
            var sixJSON = IncludeAttributes(userRepresentation, new List<SCIMAttributeExpression> { sixFilter });
            var sevenJSON = ExcludeAttributes(userRepresentation, new List<SCIMAttributeExpression> { sevenFilter }, "http://localhost");
            var eightJSON = ExcludeAttributes(userRepresentation, new List<SCIMAttributeExpression> { eightFilter }, "http://localhost");
            var nineJSON = IncludeAttributes(userRepresentation, new List<SCIMAttributeExpression> { nineFilter });
            var tenJSON = IncludeAttributes(userRepresentation, new List<SCIMAttributeExpression> { tenFilter });

            Assert.Equal("01", firstJSON.SelectToken("phones[0].phoneNumber").ToString());
            Assert.Equal("02", secondJSON.SelectToken("phones[0].phoneNumber").ToString());
            Assert.Equal("home", secondJSON.SelectToken("phones[0].type").ToString());
            Assert.Null(thirdJSON.SelectToken("userName"));
            Assert.Null(fourthJSON.SelectToken("phones[0].phoneNumber"));
            Assert.True((fifthJSON.SelectToken("phones") as JArray).Count == 1);
            Assert.NotNull(sixJSON.SelectToken("meta.lastModified"));
            Assert.Null(sevenJSON.SelectToken("meta.lastModified"));
            Assert.NotNull(eightJSON.SelectToken("id"));
            Assert.NotNull(nineJSON.SelectToken("id"));
            Assert.Equal("20", tenJSON.SelectToken("info.age").ToString());
        }

        private static JObject IncludeAttributes(SCIMRepresentation representation, IEnumerable<SCIMAttributeExpression> attributes, string location = null, bool isComplex = false)
        {
            var clone = representation.Clone() as SCIMRepresentation;
            clone.FilterAttributes(attributes, new SCIMAttributeExpression[0]);
            clone.AddStandardAttributes(location, attributes.Select(_ => _.GetFullPath()), true, false);
            return clone.ToResponse(location, false, false);
        }

        private static JObject ExcludeAttributes(SCIMRepresentation representation, IEnumerable<SCIMAttributeExpression> attributes, string location = null, bool isComplex = false)
        {
            var clone = representation.Clone() as SCIMRepresentation;
            clone.FilterAttributes(new SCIMAttributeExpression[0], attributes);
            clone.AddStandardAttributes(location, attributes.Select(_ => _.GetFullPath()), false, false);
            return clone.ToResponse(location, false, false);
        }
    }
}
