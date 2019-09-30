using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Helpers
{
    public class SCIMRepresentationHelper : ISCIMRepresentationHelper
    {
        public SCIMRepresentation ExtractSCIMRepresentationFromJSON(JObject json, IEnumerable<SCIMSchema> schemas)
        {
            return new SCIMRepresentation(schemas, ResolveAttributes(json, schemas.SelectMany(s => s.Attributes)));
        }

        private static ICollection<SCIMRepresentationAttribute> ResolveAttributes(JObject json, IEnumerable<SCIMSchemaAttribute> attrsSchema)
        {
            var missingRequiredAttributes = attrsSchema.Where(a => a.Required && !json.ContainsKey(a.Name));
            if (missingRequiredAttributes.Any())
            {
                throw new SCIMSchemaViolatedException("missingRequiredAttribute", $"required attributes {string.Join(",", missingRequiredAttributes.Select(a => a.Name))} are missing");
            }

            var result = new List<SCIMRepresentationAttribute>();
            foreach (var jsonProperty in json)
            {
                if (jsonProperty.Key == "schemas")
                {
                    continue;
                }

                var attrSchema = attrsSchema.FirstOrDefault(a => a.Name == jsonProperty.Key);
                if (attrSchema == null)
                {
                    throw new SCIMSchemaViolatedException("unrecognizedAttribute", $"attribute {jsonProperty.Key} is not recognized by the SCIM schema");
                }

                if (attrSchema.Mutability == SCIMSchemaAttributeMutabilities.READONLY)
                {
                    continue;
                }

                if (attrSchema.MultiValued)
                {
                    var jArr = jsonProperty.Value as JArray;
                    if (jArr == null)
                    {
                        throw new SCIMSchemaViolatedException("badFormatAttribute", $"attribute {jsonProperty.Key} is not an array");
                    }

                    foreach (var subJson in jArr)
                    {
                        result.Add(BuildAttribute(subJson, attrSchema));
                    }
                }
                else
                {
                    result.Add(BuildAttribute(jsonProperty.Value, attrSchema));
                }
            }

            return result;
        }

        private static SCIMRepresentationAttribute BuildAttribute(JToken jsonProperty, SCIMSchemaAttribute schemaAttribute)
        {
            var result = new SCIMRepresentationAttribute(schemaAttribute);
            switch (schemaAttribute.Type)
            {
                case SCIMSchemaAttributeTypes.BOOLEAN:
                    result.ValuesBoolean.Add(bool.Parse(jsonProperty.ToString()));
                    break;
                case SCIMSchemaAttributeTypes.INTEGER:
                    result.ValuesInteger.Add(int.Parse(jsonProperty.ToString()));
                    break;
                case SCIMSchemaAttributeTypes.DATETIME:
                    result.ValuesDateTime.Add(DateTime.Parse(jsonProperty.ToString()));
                    break;
                case SCIMSchemaAttributeTypes.STRING:
                    result.ValuesString.Add(jsonProperty.ToString());
                    break;
                case SCIMSchemaAttributeTypes.COMPLEX:
                    result.Values = ResolveAttributes(jsonProperty as JObject, schemaAttribute.SubAttributes);
                    break;
                case SCIMSchemaAttributeTypes.REFERENCE:
                    // REFERENCE.
                    break;
            }

            return result;
        }
    }
}