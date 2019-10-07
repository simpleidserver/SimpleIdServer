using Newtonsoft.Json.Linq;
using SimpleIdServer.Persistence.Filters.SCIMExpressions;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Extensions
{
    public static class SCIMExpressionExtensions
    {
        public static JObject Serialize(this ICollection<SCIMExpression> scimExpressions, SCIMRepresentation scimRepresentation)
        {
            var result = new JObject();
            foreach(var scimExpression in scimExpressions)
            {
                scimExpression.Serialize(scimRepresentation, result);
            }

            return result;
        }

        private static void Serialize(this SCIMExpression scimExpression, SCIMRepresentation scimRepresentation, JObject result)
        {
            var scimAttributeExpression = scimExpression as SCIMAttributeExpression;
            if (scimAttributeExpression == null)
            {
                throw new SCIMAttributeException("not a valid attribute expression");
            }

            scimAttributeExpression.Serialize(scimRepresentation.Attributes, result);
        }

        private static void Serialize(this SCIMAttributeExpression scimExpression, ICollection<SCIMRepresentationAttribute> attributes, JObject result)
        {
            var filteredAttributes = attributes.Where(a => a.SchemaAttribute.Name == scimExpression.Name).ToList();
            if(!filteredAttributes.Any())
            {
                return;
            }

            if (scimExpression.Child != null)
            {
                foreach(var kvp in filteredAttributes.GroupBy(f => f.SchemaAttribute.Name))
                {
                    var representationAttr = kvp.First();
                    if (representationAttr.SchemaAttribute.MultiValued == false)
                    {
                        var jObjVal = new JObject();
                        scimExpression.Child.Serialize(representationAttr.Values, jObjVal);
                        result.Add(representationAttr.SchemaAttribute.Name, jObjVal);
                    }
                    else
                    {
                        var jArr = new JArray();
                        foreach (var attr in kvp)
                        {
                            var jObjVal = new JObject();
                            scimExpression.Child.Serialize(attr.Values, jObjVal);
                            jArr.Add(jObjVal);
                        }

                        result.Add(representationAttr.SchemaAttribute.Name, jArr);
                    }
                }

                return;
            }

            EnrichResponse(filteredAttributes, result);
        }

        public static void EnrichResponse(ICollection<SCIMRepresentationAttribute> attributes, JObject jObj)
        {
            foreach (var kvp in attributes.GroupBy(a => a.SchemaAttribute.Name))
            {
                var representationAttr = kvp.First();
                if (!representationAttr.IsReadable(true))
                {
                    continue;
                }

                switch (representationAttr.SchemaAttribute.Type)
                {
                    case SCIMSchemaAttributeTypes.STRING:
                        jObj.Add(representationAttr.SchemaAttribute.Name, representationAttr.SchemaAttribute.MultiValued ? (JToken)new JArray(representationAttr.ValuesString) : representationAttr.ValuesString.First());
                        break;
                    case SCIMSchemaAttributeTypes.BOOLEAN:
                        jObj.Add(representationAttr.SchemaAttribute.Name, representationAttr.SchemaAttribute.MultiValued ? (JToken)new JArray(representationAttr.ValuesBoolean) : representationAttr.ValuesBoolean.First());
                        break;
                    case SCIMSchemaAttributeTypes.INTEGER:
                        jObj.Add(representationAttr.SchemaAttribute.Name, representationAttr.SchemaAttribute.MultiValued ? (JToken)new JArray(representationAttr.ValuesInteger) : representationAttr.ValuesInteger.First());
                        break;
                    case SCIMSchemaAttributeTypes.DATETIME:
                        jObj.Add(representationAttr.SchemaAttribute.Name, representationAttr.SchemaAttribute.MultiValued ? (JToken)new JArray(representationAttr.ValuesDateTime) : representationAttr.ValuesDateTime.First());
                        break;
                    case SCIMSchemaAttributeTypes.COMPLEX:
                        if (representationAttr.SchemaAttribute.MultiValued == false)
                        {
                            var jObjVal = new JObject();
                            EnrichResponse(representationAttr.Values, jObjVal);
                            jObj.Add(representationAttr.SchemaAttribute.Name, jObjVal);
                        }
                        else
                        {
                            var jArr = new JArray();
                            foreach (var attr in kvp)
                            {
                                var jObjVal = new JObject();
                                EnrichResponse(attr.Values, jObjVal);
                                jArr.Add(jObjVal);
                            }

                            jObj.Add(representationAttr.SchemaAttribute.Name, jArr);
                        }
                        break;
                }
            }
        }
    }
}
