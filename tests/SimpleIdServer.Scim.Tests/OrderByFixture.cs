// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Persistence.Filters;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Domains.Builders;
using SimpleIdServer.Scim.Parser;
using SimpleIdServer.Scim.Parser.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SimpleIdServer.Scim.Tests
{
    public class OrderByFixture
    {
        public static SCIMSchema CustomSchema = SCIMSchemaBuilder.Create("urn:ietf:params:scim:schemas:core:2.0:CustomProperties", "User", SCIMEndpoints.User, "Custom properties", false)
                    .AddDecimalAttribute("age")
                    .AddBinaryAttribute("eidCertificate")
                    .AddStringAttribute("filePath")
                    .Build();

        [Fact]
        public void When_Parse_And_Execute_OrderBy()
        {
            var firstRepresentation = SCIMRepresentationBuilder.Create(new List<SCIMSchema> { StandardSchemas.UserSchema, CustomSchema })
                .AddStringAttribute("userName", "urn:ietf:params:scim:schemas:core:2.0:User", new List<string> { "bjensen" })
                .AddBooleanAttribute("active", "urn:ietf:params:scim:schemas:core:2.0:User", new List<bool> { true })
                .AddDecimalAttribute("age", "urn:ietf:params:scim:schemas:core:2.0:CustomProperties", new List<decimal> { 22 })
                .AddBinaryAttribute("eidCertificate", "urn:ietf:params:scim:schemas:core:2.0:CustomProperties", new List<string> { "aGVsbG8=" })
                .AddStringAttribute("title", "urn:ietf:params:scim:schemas:core:2.0:User", new List<string> { "title" })
                .AddStringAttribute("userType", "urn:ietf:params:scim:schemas:core:2.0:User", new List<string> { "Employee" })
                .AddStringAttribute("filePath", "urn:ietf:params:scim:schemas:core:2.0:CustomProperties", new List<string> { @"C:\Program Files (x86)\Internet Explorer" })
                .AddComplexAttribute("emails", "urn:ietf:params:scim:schemas:core:2.0:User", (c) =>
                {
                    c.AddStringAttribute("value", new List<string> { "example.com" });
                    c.AddStringAttribute("type", new List<string> { "work" });
                })
                .AddComplexAttribute("emails", "urn:ietf:params:scim:schemas:core:2.0:User", (c) =>
                {
                    c.AddStringAttribute("value", new List<string> { "example.org" });
                })
                .AddComplexAttribute("name", "urn:ietf:params:scim:schemas:core:2.0:User", (r) =>
                {
                    r.AddStringAttribute("familyName", new List<string> { "O'Malley" });
                })
                .Build();
            firstRepresentation.LastModified = DateTime.Parse("2012-05-13T04:42:34Z");
            firstRepresentation.Version = "2";
            var secondRepresentation = SCIMRepresentationBuilder.Create(new List<SCIMSchema> { StandardSchemas.UserSchema })
                .AddStringAttribute("userName", "urn:ietf:params:scim:schemas:core:2.0:User", new List<string> { "Justine" })
                .AddStringAttribute("title", "urn:ietf:params:scim:schemas:core:2.0:User", new List<string> { "title" })
                .AddStringAttribute("userType", "urn:ietf:params:scim:schemas:core:2.0:User", new List<string> { "Intern" })
                .AddComplexAttribute("ims", "urn:ietf:params:scim:schemas:core:2.0:User", (c) =>
                {
                    c.AddStringAttribute("type", new List<string> { "xmpp" });
                    c.AddStringAttribute("value", new List<string> { "foo.com" });
                })
                .AddComplexAttribute("emails", "urn:ietf:params:scim:schemas:core:2.0:User", (c) =>
                {
                    c.AddStringAttribute("value", new List<string> { "example.be" });
                })
                .Build();
            secondRepresentation.LastModified = DateTime.Parse("2010-05-13T04:42:34Z");
            var representations = new List<SCIMRepresentation>
            {
                firstRepresentation,
                secondRepresentation
            };

            var reps = representations.OrderBy(s => s.FlatAttributes.Where(a => a.FullPath == StandardSchemas.UserSchema.GetAttribute("userName").Id).FirstOrDefault()).ToList();

            var firstResult = ParseAndExecutOrderBy(representations.AsQueryable(), "emails.value", SearchSCIMRepresentationOrders.Ascending);
            var secondResult = ParseAndExecutOrderBy(representations.AsQueryable(), "emails.value", SearchSCIMRepresentationOrders.Descending);
            var thirdResult = ParseAndExecutOrderBy(representations.AsQueryable(), "userName", SearchSCIMRepresentationOrders.Ascending);
            var fourthResult = ParseAndExecutOrderBy(representations.AsQueryable(), "userName", SearchSCIMRepresentationOrders.Descending);
            var firthResult = ParseAndExecutOrderBy(representations.AsQueryable(), "meta.lastModified", SearchSCIMRepresentationOrders.Ascending);
            var sixResult = ParseAndExecutOrderBy(representations.AsQueryable(), "meta.lastModified", SearchSCIMRepresentationOrders.Descending);

            Assert.Equal("Justine", firstResult.ElementAt(0).FlatAttributes.First(a => a.SchemaAttribute.Name == "userName").ValueString);
            Assert.Equal("bjensen", firstResult.ElementAt(1).FlatAttributes.First(a => a.SchemaAttribute.Name == "userName").ValueString);
            Assert.Equal("bjensen", secondResult.ElementAt(0).FlatAttributes.First(a => a.SchemaAttribute.Name == "userName").ValueString);
            Assert.Equal("Justine", secondResult.ElementAt(1).FlatAttributes.First(a => a.SchemaAttribute.Name == "userName").ValueString);
            Assert.Equal("bjensen", thirdResult.ElementAt(0).FlatAttributes.First(a => a.SchemaAttribute.Name == "userName").ValueString);
            Assert.Equal("Justine", thirdResult.ElementAt(1).FlatAttributes.First(a => a.SchemaAttribute.Name == "userName").ValueString);
            Assert.Equal("Justine", fourthResult.ElementAt(0).FlatAttributes.First(a => a.SchemaAttribute.Name == "userName").ValueString);
            Assert.Equal("bjensen", fourthResult.ElementAt(1).FlatAttributes.First(a => a.SchemaAttribute.Name == "userName").ValueString);
            Assert.Equal("Justine", firthResult.ElementAt(0).FlatAttributes.First(a => a.SchemaAttribute.Name == "userName").ValueString);
            Assert.Equal("bjensen", firthResult.ElementAt(1).FlatAttributes.First(a => a.SchemaAttribute.Name == "userName").ValueString);
            Assert.Equal("bjensen", sixResult.ElementAt(0).FlatAttributes.First(a => a.SchemaAttribute.Name == "userName").ValueString);
            Assert.Equal("Justine", sixResult.ElementAt(1).FlatAttributes.First(a => a.SchemaAttribute.Name == "userName").ValueString);
        }

        private IOrderedEnumerable<SCIMRepresentation> ParseAndExecutOrderBy(IQueryable<SCIMRepresentation> representations, string filter, SearchSCIMRepresentationOrders order)
        {
            var parsed = SCIMFilterParser.Parse(filter, new List<SCIMSchema> { StandardSchemas.UserSchema, CustomSchema });
            var evaluatedExpression = parsed.EvaluateOrderBy(representations, order);
            return (IOrderedEnumerable<SCIMRepresentation>)evaluatedExpression.Compile().DynamicInvoke(representations);
        }
    }
}
