using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Builder;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Helpers;
using System.Collections.Generic;
using Xunit;

namespace SimpleIdServer.Scim.Tests.Helpers
{
    public class SCIMRepresentationHelperFixture
    {
        [Fact]
        public void When_Extract_Scim_Representation_From_Json()
        {
            var helper = new SCIMRepresentationHelper();
            var json = JObject.Parse("{"+
                "'schemas':['urn:ietf:params:scim:schemas:core:2.0:User'],"+
                "'userName':'bjensen',"+
                "'name':{"+
                    "'formatted':'Ms. Barbara J Jensen III',"+
                    "'familyName':'Jensen'," +
                    "'givenName':'Barbara'" +
                "}"+
           "}");
            var schemaBuilder = new SCIMSchemaBuilder("urn:ietf:params:scim:schemas:core:2.0:User", "User", "User Account")
                .AddAttribute("userName", o =>
                {
                    o.SetType(SCIMSchemaAttributeTypes.STRING);
                    o.SetMultiValued(false);
                    o.SetRequired(true);
                    o.SetCaseExact(true);
                    o.SetMutability(SCIMSchemaAttributeMutabilities.READWRITE);
                    o.SetReturned(SCIMSchemaAttributeReturned.DEFAULT);
                    o.SetUniqueness(SCIMSchemaAttributeUniqueness.SERVER);
                })
                .AddAttribute("name", o =>
                {
                    o.SetType(SCIMSchemaAttributeTypes.COMPLEX);
                    o.SetMultiValued(false);
                    o.SetRequired(false);
                    o.SetMutability(SCIMSchemaAttributeMutabilities.WRITEONLY);
                    o.AddAttribute("formatted", opt =>
                    {
                        opt.SetType(SCIMSchemaAttributeTypes.STRING);
                        opt.SetMultiValued(false);
                        opt.SetRequired(false);
                        opt.SetCaseExact(false);
                        opt.SetMutability(SCIMSchemaAttributeMutabilities.READWRITE);
                    });
                    o.AddAttribute("familyName", opt =>
                    {
                        opt.SetType(SCIMSchemaAttributeTypes.STRING);
                        opt.SetMultiValued(false);
                        opt.SetRequired(false);
                        opt.SetCaseExact(false);
                        opt.SetMutability(SCIMSchemaAttributeMutabilities.READWRITE);
                    });
                    o.AddAttribute("givenName", opt =>
                    {
                        opt.SetType(SCIMSchemaAttributeTypes.STRING);
                        opt.SetMultiValued(false);
                        opt.SetRequired(false);
                        opt.SetCaseExact(false);
                        opt.SetMutability(SCIMSchemaAttributeMutabilities.READWRITE);
                    });
                });
            var representation = helper.ExtractSCIMRepresentationFromJSON(json, new List<SCIMSchema>
            {
                schemaBuilder.Build()
            });
            string s = "";
        }
    }
}
