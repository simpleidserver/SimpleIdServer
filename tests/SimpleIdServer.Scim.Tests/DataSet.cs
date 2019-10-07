using SimpleIdServer.Scim.Domain;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Tests
{
    public class DataSet
    {
        public static List<SCIMRepresentation> GetSCIMRepresentations()
        {
            var userNameAttribute = new SCIMRepresentationAttribute
            {
                SchemaAttribute = new SCIMSchemaAttribute(Guid.NewGuid().ToString())
                {
                    Name = "userName",
                    Type = SCIMSchemaAttributeTypes.STRING,
                    MultiValued = false
                }
            };
            var firstBrandAttribute = new SCIMRepresentationAttribute
            {
                SchemaAttribute = new SCIMSchemaAttribute(Guid.NewGuid().ToString())
                {
                    Name = "brand",
                    Type = SCIMSchemaAttributeTypes.STRING,
                    MultiValued = false
                }
            };
            var secondBrandAttribute = new SCIMRepresentationAttribute
            {
                SchemaAttribute = new SCIMSchemaAttribute(Guid.NewGuid().ToString())
                {
                    Name = "brand",
                    Type = SCIMSchemaAttributeTypes.STRING,
                    MultiValued = false
                }
            };
            var ageAttribute = new SCIMRepresentationAttribute
            {
                SchemaAttribute = new SCIMSchemaAttribute(Guid.NewGuid().ToString())
                {
                    Name = "age",
                    Type = SCIMSchemaAttributeTypes.INTEGER,
                    MultiValued = false
                }
            };
            var frenchName = new SCIMRepresentationAttribute
            {
                SchemaAttribute = new SCIMSchemaAttribute(Guid.NewGuid().ToString())
                {
                    Name = "FR",
                    Type = SCIMSchemaAttributeTypes.STRING
                }
            };
            var languages = new SCIMRepresentationAttribute
            {
                SchemaAttribute = new SCIMSchemaAttribute(Guid.NewGuid().ToString())
                {
                    Name = "languages",
                    Type = SCIMSchemaAttributeTypes.COMPLEX
                },
                Values = new List<SCIMRepresentationAttribute>
                {
                    frenchName
                }
            };
            var translationNames = new SCIMRepresentationAttribute
            {
                SchemaAttribute = new SCIMSchemaAttribute(Guid.NewGuid().ToString())
                {
                    Name = "translations",
                    Type = SCIMSchemaAttributeTypes.COMPLEX
                },
                Values = new List<SCIMRepresentationAttribute>
                {
                    languages
                }
            };
            var formattedNameAttribute = new SCIMRepresentationAttribute
            {
                SchemaAttribute = new SCIMSchemaAttribute(Guid.NewGuid().ToString())
                {
                    Name = "formatted",
                    Type = SCIMSchemaAttributeTypes.STRING
                }
            };
            var nameAttribute = new SCIMRepresentationAttribute
            {
                SchemaAttribute = new SCIMSchemaAttribute(Guid.NewGuid().ToString())
                {
                    Name = "name",
                    Type = SCIMSchemaAttributeTypes.COMPLEX
                },
                Values = new List<SCIMRepresentationAttribute>
                {
                    formattedNameAttribute,
                    translationNames
                }
            };
            var firstMobilePhoneValueAttribute = new SCIMRepresentationAttribute
            {
                SchemaAttribute = new SCIMSchemaAttribute(Guid.NewGuid().ToString())
                {
                    Name = "value",
                    Type = SCIMSchemaAttributeTypes.STRING,
                    MultiValued = false
                }
            };
            var secondMobilePhoneValueAttribute = new SCIMRepresentationAttribute
            {
                SchemaAttribute = new SCIMSchemaAttribute(Guid.NewGuid().ToString())
                {
                    Name = "value",
                    Type = SCIMSchemaAttributeTypes.STRING,
                    MultiValued = false
                }
            };
            var firstMobilePhoneAttribute = new SCIMRepresentationAttribute
            {
                SchemaAttribute = new SCIMSchemaAttribute(Guid.NewGuid().ToString())
                {
                    Name = "mobile",
                    Type = SCIMSchemaAttributeTypes.COMPLEX,
                    MultiValued = false
                },
                Values = new List<SCIMRepresentationAttribute>
                {
                    firstMobilePhoneValueAttribute
                }
            };
            var secondMobilePhoneAttribute = new SCIMRepresentationAttribute
            {
                SchemaAttribute = new SCIMSchemaAttribute(Guid.NewGuid().ToString())
                {
                    Name = "mobile",
                    Type = SCIMSchemaAttributeTypes.COMPLEX,
                    MultiValued = false
                },
                Values = new List<SCIMRepresentationAttribute>
                {
                    secondMobilePhoneValueAttribute
                }
            };
            var phoneAttribute = new SCIMRepresentationAttribute
            {
                SchemaAttribute = new SCIMSchemaAttribute(Guid.NewGuid().ToString())
                {
                    Name = "phone",
                    Type = SCIMSchemaAttributeTypes.COMPLEX,
                    MultiValued = true
                },
                Values = new List<SCIMRepresentationAttribute>
                {
                    firstMobilePhoneAttribute,
                    secondMobilePhoneAttribute,
                    firstBrandAttribute
                }
            };
            userNameAttribute.Add("jsmith");
            ageAttribute.Add(25);
            formattedNameAttribute.Add("formatted");
            frenchName.Add("john");
            firstMobilePhoneValueAttribute.Add("01");
            secondMobilePhoneValueAttribute.Add("02");
            firstBrandAttribute.Add("samsung");
            secondBrandAttribute.Add("iphone");
            var personRepresentation = new SCIMRepresentation
            {
                Attributes = new List<SCIMRepresentationAttribute>
                {
                    userNameAttribute,
                    ageAttribute,
                    nameAttribute,
                    phoneAttribute
                }
            };
            var representations = new List<SCIMRepresentation>
            {
                personRepresentation
            };
            return representations;
        }
    }
}
