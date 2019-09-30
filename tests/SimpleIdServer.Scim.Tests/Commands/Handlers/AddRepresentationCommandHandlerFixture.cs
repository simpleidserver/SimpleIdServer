using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Builder;
using SimpleIdServer.Scim.Commands;
using SimpleIdServer.Scim.Commands.Handlers;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Persistence.InMemory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SimpleIdServer.Scim.Tests.Commands.Handlers
{
    public class AddRepresentationCommandHandlerFixture
    {
        // [Fact]
        public async Task When_Add_Representation()
        {
            var json = JObject.Parse("{" +
                "'schemas':['urn:ietf:params:scim:schemas:core:2.0:User']," +
                "'userName':'bjensen'," +
                "'name':{" +
                    "'formatted':'Ms. Barbara J Jensen III'," +
                    "'familyName':'Jensen'," +
                    "'givenName':'Barbara'" +
                "}" +
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
            var schema = schemaBuilder.Build();
            var representations = new List<SCIMRepresentation>
            {
                new SCIMRepresentation
                {
                    ResourceType = "User",
                    Schemas = new List<SCIMSchema>
                    {
                        schema
                    },
                    Attributes = new List<SCIMRepresentationAttribute>
                    {
                        new SCIMRepresentationAttribute
                        {
                            SchemaAttribute = schema.Attributes.First(a => a.Name == "userName"),
                            ValuesString = new List<string>
                            {
                                "bjensen"
                            }
                        }
                        
                    }
                }
            };
            var defaultSchemaQueryRepository = new DefaultSchemaQueryRepository(new List<SCIMSchema>
            {
                schemaBuilder.Build()
            });
            var scimRepresentationQueryRepository = new DefaultSCIMRepresentationQueryRepository(representations);
            var scimRepresentationCommandRepository = new DefaultSCIMRepresentationCommandRepository(representations);
            var scimRepresentationHelper = new SCIMRepresentationHelper();
            var handler = new AddRepresentationCommandHandler(defaultSchemaQueryRepository, scimRepresentationQueryRepository, scimRepresentationHelper, scimRepresentationCommandRepository);
            await handler.Handle(new AddRepresentationCommand("User", new List<string>
            {
                "urn:ietf:params:scim:schemas:core:2.0:User"
            }, json));
        }
    }
}
