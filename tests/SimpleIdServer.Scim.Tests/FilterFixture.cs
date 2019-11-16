// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Builder;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SimpleIdServer.Scim.Tests
{
    public class FilterFixture
    {
        [Fact]
        public void When_Parse_And_Execute_Filter()
        {
            var firstRepresentation = SCIMRepresentationBuilder.Create(new List<SCIMSchema> { SCIMConstants.StandardSchemas.UserSchema, SCIMConstants.StandardSchemas.CommonSchema })
                .AddStringAttribute("userName", "urn:ietf:params:scim:schemas:core:2.0:User", new List<string> { "bjensen" })
                .AddBooleanAttribute("active", "urn:ietf:params:scim:schemas:core:2.0:User", new List<bool> { true })
                .AddStringAttribute("title", "urn:ietf:params:scim:schemas:core:2.0:User", new List<string> { "title" })
                .AddStringAttribute("userType", "urn:ietf:params:scim:schemas:core:2.0:User", new List<string> { "Employee" })
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
                .AddComplexAttribute("meta", SCIMConstants.StandardSchemas.CommonSchema.Id, (r) =>
                {
                    r.AddDateTimeAttribute("lastModified", new List<DateTime> { DateTime.Parse("2012-05-13T04:42:34Z") });
                })
                .Build();
            var secondRepresentation = SCIMRepresentationBuilder.Create(new List<SCIMSchema> { SCIMConstants.StandardSchemas.UserSchema, SCIMConstants.StandardSchemas.CommonSchema })
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
                .AddComplexAttribute("meta", SCIMConstants.StandardSchemas.CommonSchema.Id, (r) =>
                {
                    r.AddDateTimeAttribute("lastModified", new List<DateTime> { DateTime.Parse("2010-05-13T04:42:34Z") });
                })
                .Build();
            secondRepresentation.LastModified = DateTime.Now;
            var representations = new List<SCIMRepresentation>
            {
                firstRepresentation,
                secondRepresentation
            };

            var otherResult = ParseAndExecuteFilter(representations.AsQueryable(), "emails co \"example.com\"");
            var firstResult = ParseAndExecuteFilter(representations.AsQueryable(), "userName eq \"bjensen\"");
            var secondResult = ParseAndExecuteFilter(representations.AsQueryable(), "name.familyName co \"O'Malley\"");
            var thirdResult = ParseAndExecuteFilter(representations.AsQueryable(), "userName sw \"J\"");
            var fourthResult = ParseAndExecuteFilter(representations.AsQueryable(), "title pr");
            var fifthResult = ParseAndExecuteFilter(representations.AsQueryable(), "meta.lastModified gt \"2011-05-13T04:42:34Z\"");
            var sixResult = ParseAndExecuteFilter(representations.AsQueryable(), "meta.lastModified ge \"2011-05-13T04:42:34Z\"");
            var sevenResult = ParseAndExecuteFilter(representations.AsQueryable(), "meta.lastModified lt \"2011-05-13T04:42:34Z\"");
            var eightResult = ParseAndExecuteFilter(representations.AsQueryable(), "meta.lastModified le \"2011-05-13T04:42:34Z\"");
            var nineResult = ParseAndExecuteFilter(representations.AsQueryable(), "title pr and userType eq \"Employee\"");
            var tenResult = ParseAndExecuteFilter(representations.AsQueryable(), "title pr or userType eq \"Intern\"");
            var elevenResult = ParseAndExecuteFilter(representations.AsQueryable(), "userType eq \"Employee\" and (emails.value co \"example.org\" or emails.value co \"example.org\")");
            var twelveResult = ParseAndExecuteFilter(representations.AsQueryable(), "userType ne \"Employee\" and not (emails co \"example.com\" or emails.value co \"example.org\")");
            var thirteenResult = ParseAndExecuteFilter(representations.AsQueryable(), "userType eq \"Employee\" and (emails.type eq \"work\")");
            var fourteenResult = ParseAndExecuteFilter(representations.AsQueryable(), "userType eq \"Employee\" and emails[type eq \"work\" and value co \"example.com\"]");
            var fifteenResult = ParseAndExecuteFilter(representations.AsQueryable(), "emails[type eq \"work\" and value co \"example.com\"] or ims[type eq \"xmpp\" and value co \"foo.com\"]");
            
            Assert.Equal(1, firstResult.Count());
            Assert.Equal(1, secondResult.Count());
            Assert.Equal(1, thirdResult.Count());
            Assert.Equal(2, fourthResult.Count());
            Assert.Equal(1, fifthResult.Count());
            Assert.Equal(1, sixResult.Count());
            Assert.Equal(1, sevenResult.Count());
            Assert.Equal(1, eightResult.Count());
            Assert.Equal(1, nineResult.Count());
            Assert.Equal(2, tenResult.Count());
            Assert.Equal(1, elevenResult.Count());
            Assert.Equal(1, twelveResult.Count());
            Assert.Equal(1, thirteenResult.Count());
            Assert.Equal(1, fourteenResult.Count());
            Assert.Equal(2, fifteenResult.Count());
        }

        private IQueryable<SCIMRepresentation> ParseAndExecuteFilter(IQueryable<SCIMRepresentation> representations, string filter)
        {
            var parsed = SCIMFilterParser.Parse(filter, new List<SCIMSchema> { SCIMConstants.StandardSchemas.UserSchema, SCIMConstants.StandardSchemas.CommonSchema });
            var evaluatedExpression = parsed.Evaluate(representations);
            return (IQueryable<SCIMRepresentation>)evaluatedExpression.Compile().DynamicInvoke(representations);
        }
    }
}
