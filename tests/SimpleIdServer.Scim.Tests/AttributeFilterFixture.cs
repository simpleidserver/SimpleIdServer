using SimpleIdServer.Persistence.Filters.SCIMExpressions;
using SimpleIdServer.Scim.Builder;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Helpers;
using System.Collections.Generic;
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

            var firstFilter = SCIMFilterParser.Parse("phones.phoneNumber");

            var firstJson = (new List<SCIMExpression> { firstFilter }).Serialize(userRepresentation);

            Assert.Equal("01", firstJson.SelectToken("phones[0].phoneNumber").ToString());
        }
    }
}
