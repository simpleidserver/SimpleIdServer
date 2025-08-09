// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
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
    public class FilterFixture
    {
        [Fact]
        public void When_Parse_And_Execute_Filter()
        {
            var customSchema = SCIMSchemaBuilder.Create("urn:ietf:params:scim:schemas:core:2.0:CustomProperties", "User", SCIMEndpoints.User, "Custom properties", false)
                    .AddDecimalAttribute("age")
                    .AddBinaryAttribute("eidCertificate")
                    .AddStringAttribute("filePath")
                    .Build();
            var schema = StandardSchemas.UserSchema;
            var representation = new SCIMRepresentation
            {
                Id = Guid.NewGuid().ToString(),
                LastModified = DateTime.Parse("2010-05-13T04:42:34Z")
            };
            var secondRepresentation = new SCIMRepresentation
            {
                Id = Guid.NewGuid().ToString(),
                LastModified = DateTime.Parse("2012-05-13T04:42:34Z"),
                Version = "2"
            };
            var thirdRepresentation = new SCIMRepresentation
            {
                Id = Guid.NewGuid().ToString()
            };
            var fourthRepresentation = new SCIMRepresentation
            {
                Id = Guid.NewGuid().ToString(),
                LastModified = DateTime.Parse("2010-05-13T04:42:34Z")
            };
            representation.AddAttribute(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schema.GetAttribute("title"), schema.Id)
            {
                Id = Guid.NewGuid().ToString(),
                ValueString = "title"
            });
            representation.AddAttribute(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), customSchema.GetAttribute("age"), customSchema.Id)
            {
                Id = Guid.NewGuid().ToString(),
                ValueDecimal = 20
            });
            representation.AddAttribute(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), customSchema.GetAttribute("eidCertificate"), customSchema.Id)
            {
                Id = Guid.NewGuid().ToString(),
                ValueBinary = "aGVsbG8="
            });
            representation.AddAttribute(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), customSchema.GetAttribute("filePath"), customSchema.Id)
            {
                Id = Guid.NewGuid().ToString(),
                ValueString = @"C:\Program Files (x86)\Internet Explorer"
            });
            representation.AddAttribute(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schema.GetAttribute("userType"), schema.Id)
            {
                Id = Guid.NewGuid().ToString(),
                ValueString = "Employee"
            });
            representation.AddAttribute(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schema.GetAttribute("emails"), schema.Id));
            representation.AddAttribute(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schema.GetAttribute("emails.value"), schema.Id)
            {
                Id = Guid.NewGuid().ToString(),
                ValueString = "example.org"
            });
            representation.AddAttribute(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schema.GetAttribute("emails.type"), schema.Id)
            {
                Id = Guid.NewGuid().ToString(),
                ValueString = "work"
            });
            representation.AddAttribute(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schema.GetAttribute("phoneNumbers"), schema.Id));
            representation.AddAttribute(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schema.GetAttribute("phoneNumbers.primary"), schema.Id)
            {
                ValueBoolean = true
            });
            representation.AddAttribute(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schema.GetAttribute("userName"), schema.Id)
            {
                Id = Guid.NewGuid().ToString(),
                ValueString = "bjensen"
            });
            representation.AddAttribute(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schema.GetAttribute("name.familyName"), schema.Id)
            {
                Id = Guid.NewGuid().ToString(),
                ValueString = "O'Malley"
            });
            secondRepresentation.AddAttribute(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schema.GetAttribute("userName"), schema.Id)
            {
                Id = Guid.NewGuid().ToString(),
                ValueString = "Justine"
            });
            secondRepresentation.AddAttribute(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schema.GetAttribute("userType"), schema.Id)
            {
                Id = Guid.NewGuid().ToString(),
                ValueString = "Intern"
            });
            thirdRepresentation.AddAttribute(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schema.GetAttribute("userName"), schema.Id)
            {
                Id = Guid.NewGuid().ToString(),
                ValueString = "Jule"
            });
            thirdRepresentation.AddAttribute(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schema.GetAttribute("ims"), schema.Id));
            thirdRepresentation.AddAttribute(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schema.GetAttribute("ims.type"), schema.Id)
            {
                ValueString = "xmpp"
            });
            thirdRepresentation.AddAttribute(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schema.GetAttribute("ims.value"), schema.Id)
            {
                ValueString = "foo.com"
            });
            fourthRepresentation.AddAttribute(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schema.GetAttribute("userName"), schema.Id)
            {
                Id = Guid.NewGuid().ToString(),
                ValueString = "suraj"
            });
            fourthRepresentation.AddAttribute(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schema.GetAttribute("userType"), schema.Id)
            {
                Id = Guid.NewGuid().ToString(),
                ValueString = "OtherValue"
            });
            var representations = new List<SCIMRepresentation>
            {
                representation,
                secondRepresentation,
                thirdRepresentation,
                fourthRepresentation
            };

            var firstResult = ParseAndExecuteFilter(representations.AsQueryable(), "userName eq \"bjensen\"", customSchema);
            var secondResult = ParseAndExecuteFilter(representations.AsQueryable(), "name.familyName co \"O'Malley\"", customSchema);
            var thirdResult = ParseAndExecuteFilter(representations.AsQueryable(), "userName sw \"J\"", customSchema);
            var fourthResult = ParseAndExecuteFilter(representations.AsQueryable(), "title pr", customSchema);
            var fifthResult = ParseAndExecuteFilter(representations.AsQueryable(), "meta.lastModified gt \"2011-05-13T04:42:34Z\"", customSchema);
            var sixResult = ParseAndExecuteFilter(representations.AsQueryable(), "meta.lastModified ge \"2011-05-13T04:42:34Z\"", customSchema);
            var sevenResult = ParseAndExecuteFilter(representations.AsQueryable(), "meta.lastModified lt \"2011-05-13T04:42:34Z\"", customSchema);
            var eightResult = ParseAndExecuteFilter(representations.AsQueryable(), "meta.lastModified le \"2011-05-13T04:42:34Z\"", customSchema);
            var nineResult = ParseAndExecuteFilter(representations.AsQueryable(), "title pr and userType eq \"Employee\"", customSchema);
            var tenResult = ParseAndExecuteFilter(representations.AsQueryable(), "title pr or userType eq \"Intern\"", customSchema);
            var elevenResult = ParseAndExecuteFilter(representations.AsQueryable(), "userType eq \"Employee\" and (emails.value co \"example.org\" or emails.value co \"example.org\")", customSchema);
            var twelveResult = ParseAndExecuteFilter(representations.AsQueryable(), "userType ne \"Employee\" and not (emails co \"example.com\" or emails.value co \"example.org\")", customSchema);
            var thirteenResult = ParseAndExecuteFilter(representations.AsQueryable(), "userType eq \"Employee\" and (emails.type eq \"work\")", customSchema);
            var fourteenResult = ParseAndExecuteFilter(representations.AsQueryable(), "userType eq \"Employee\" and emails[type eq \"work\" and value co \"example.org\"]", customSchema);
            var fifteenResult = ParseAndExecuteFilter(representations.AsQueryable(), "emails[type eq \"work\" and value co \"example.org\"] or ims[type eq \"xmpp\" and value co \"foo.com\"]", customSchema);
            var sixteenResult = ParseAndExecuteFilter(representations.AsQueryable(), "meta.lastModified gt \"2011-05-13T04:42:34Z\" and meta.version eq \"2\"", customSchema);
            var seventeenResult = ParseAndExecuteFilter(representations.AsQueryable(), "phoneNumbers[primary eq \"true\"]", customSchema);
            var eighteenResult = ParseAndExecuteFilter(representations.AsQueryable(), "age gt 15", customSchema);
            var nineteenResult = ParseAndExecuteFilter(representations.AsQueryable(), "eidCertificate eq \"aGVsbG8=\"", customSchema);
            var twentyResult = ParseAndExecuteFilter(representations.AsQueryable(), "filePath eq \"C:\\Program Files (x86)\\Internet Explorer\"", customSchema);
            var twentyOneResult = ParseAndExecuteFilter(representations.AsQueryable(), "urn:ietf:params:scim:schemas:core:2.0:User:userName eq \"bjensen\"", customSchema);
            var thirtyResult = ParseAndExecuteFilter(representations.AsQueryable(), "urn:ietf:params:scim:schemas:core:2.0:User:name.familyName co \"O'Malley\"", customSchema);
            var twentyTwoResult = ParseAndExecuteFilter(representations.AsQueryable(), "(userName eq \"bjensen\")", customSchema);
            var twentyThreeResult = ParseAndExecuteFilter(representations.AsQueryable(), "(userName eq \"bjensen\" or userName eq \"Jule\")", customSchema);
            var twentyFourResult = ParseAndExecuteFilter(representations.AsQueryable(), "(userName eq \"bjensen\") and emails[type eq \"work\" and value co \"example.org\"] and phoneNumbers[primary eq \"true\"]", customSchema);
            var twentyFiveResult = ParseAndExecuteFilter(representations.AsQueryable(), "(username eq suraj) and ((userType eq OtherValue) or (userType eq Public))", customSchema);

            Assert.Equal(1, firstResult.Count());
            Assert.Equal(1, secondResult.Count());
            Assert.Equal(2, thirdResult.Count());
            Assert.Equal(1, fourthResult.Count());
            Assert.Equal(1, fifthResult.Count());
            Assert.Equal(1, sixResult.Count());
            Assert.Equal(3, sevenResult.Count());
            Assert.Equal(3, eightResult.Count());
            Assert.Equal(1, nineResult.Count());
            Assert.Equal(2, tenResult.Count());
            Assert.Equal(1, elevenResult.Count());
            Assert.Equal(2, twelveResult.Count());
            Assert.Equal(1, thirteenResult.Count());
            Assert.Equal(1, fourteenResult.Count());
            Assert.Equal(2, fifteenResult.Count());
            Assert.Equal(1, sixteenResult.Count());
            Assert.Equal(1, seventeenResult.Count());
            Assert.Equal(1, eighteenResult.Count());
            Assert.Equal(1, nineteenResult.Count());
            Assert.Equal(1, twentyResult.Count());
            Assert.Equal(1, twentyOneResult.Count());
            Assert.Equal(1, thirtyResult.Count());
            Assert.Equal(1, twentyTwoResult.Count());
            Assert.Equal(2, twentyThreeResult.Count());
            Assert.Equal(1, twentyFourResult.Count());
            Assert.Equal(1, twentyFiveResult.Count());
            string ss = "";
        }

        private IQueryable<SCIMRepresentation> ParseAndExecuteFilter(IQueryable<SCIMRepresentation> representations, string filter, SCIMSchema customSchema)
        {
            var parsed = SCIMFilterParser.Parse(filter, new List<SCIMSchema> { StandardSchemas.UserSchema, customSchema });
            var evaluatedExpression = parsed.Evaluate(representations);
            return (IQueryable<SCIMRepresentation>)evaluatedExpression.Compile().DynamicInvoke(representations);
        }
    }
}
