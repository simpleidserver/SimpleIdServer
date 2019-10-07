using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Domain
{
    public static class SCIMRepresentationExtensions
    {
        public static JObject ToResponse(this SCIMRepresentation representation, string location, bool isGetRequest = false)
        {
            var jObj = new JObject
            {
                { SCIMConstants.StandardSCIMRepresentationAttributes.Id, representation.Id },
                { SCIMConstants.StandardSCIMRepresentationAttributes.Schemas, new JArray(representation.Schemas.Select(s => s.Id)) },
                { SCIMConstants.StandardSCIMRepresentationAttributes.Meta, new JObject
                {
                    { SCIMConstants.StandardSCIMMetaAttributes.Created, representation.Created },
                    { SCIMConstants.StandardSCIMMetaAttributes.LastModified, representation.LastModified },
                    { SCIMConstants.StandardSCIMMetaAttributes.ResourceType, representation.ResourceType },
                    { SCIMConstants.StandardSCIMMetaAttributes.Version, representation.Version },
                    { SCIMConstants.StandardSCIMMetaAttributes.Location, location }
                } }
            };

            if (!string.IsNullOrWhiteSpace(representation.ExternalId))
            {
                jObj.Add(SCIMConstants.StandardSCIMRepresentationAttributes.ExternalId, representation.ExternalId);
            }

            EnrichResponse(representation.Attributes, jObj, isGetRequest);
            return jObj;
        }

        public static void EnrichResponse(ICollection<SCIMRepresentationAttribute> attributes, JObject jObj, bool isGetRequest = false)
        {
            foreach (var kvp in attributes.GroupBy(a => a.SchemaAttribute.Name))
            {
                var representationAttr = kvp.First();
                if (!representationAttr.IsReadable(isGetRequest))
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
                            EnrichResponse(representationAttr.Values, jObjVal, isGetRequest);
                            jObj.Add(representationAttr.SchemaAttribute.Name, jObjVal);
                        }
                        else
                        {
                            var jArr = new JArray();
                            foreach(var attr in kvp)
                            {
                                var jObjVal = new JObject();
                                EnrichResponse(representationAttr.Values, jObjVal, isGetRequest);
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
