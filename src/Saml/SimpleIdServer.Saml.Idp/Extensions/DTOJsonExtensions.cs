// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Idp.Domains;
using SimpleIdServer.Saml.Idp.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Saml.Idp.Extensions
{
    public static class DTOJsonExtensions
    {
        public static JObject ToDto(this RelyingPartyAggregate rp)
        {
            var mappings = rp.ClaimMappings.Select(c => c.ToDto());
            return new JObject
            {
                { RelyingPartyParameters.Id, rp.Id },
                { RelyingPartyParameters.AssertionExpirationTimeInSeconds, rp.AssertionExpirationTimeInSeconds },
                { RelyingPartyParameters.ClaimMappings, new JArray(mappings) },
                { RelyingPartyParameters.CreateDateTime, rp.CreateDateTime },
                { RelyingPartyParameters.MetadataUrl, rp.MetadataUrl },
                { RelyingPartyParameters.UpdateDateTime, rp.UpdateDateTime }
            };
        }

        public static JObject ToDto(this RelyingPartyClaimMapping mapping)
        {
            return new JObject
            {
                { RelyingPartyClaimMappingParameters.ClaimFormat, mapping.ClaimFormat },
                { RelyingPartyClaimMappingParameters.ClaimName, mapping.ClaimName },
                { RelyingPartyClaimMappingParameters.UserAttribute, mapping.UserAttribute }
            };
        }

        public static CreateRelyingPartyParameter ToCreateRelyingPartyParameter(this JObject jObj)
        {
            return new CreateRelyingPartyParameter
            {
                MetadataUrl = jObj.GetStr(RelyingPartyParameters.MetadataUrl)
            };
        }

        public static UpdateRelyingPartyParameter ToUpdateRelyingPartyParameter(this JObject jObj)
        {
            var mappings = new List<RelyingPartyClaimMapping>();
            var jArr = jObj.GetToken(RelyingPartyParameters.ClaimMappings) as JArray;
            if (jArr != null)
            {
                foreach(JObject record in jArr)
                {
                    mappings.Add(new RelyingPartyClaimMapping
                    {
                        ClaimFormat = record.GetStr(RelyingPartyClaimMappingParameters.ClaimFormat),
                        ClaimName = record.GetStr(RelyingPartyClaimMappingParameters.ClaimName),
                        UserAttribute = record.GetStr(RelyingPartyClaimMappingParameters.UserAttribute)
                    });
                }
            }

            return new UpdateRelyingPartyParameter
            {
                MetadataUrl = jObj.GetStr(RelyingPartyParameters.MetadataUrl),
                AssertionExpirationTimeInSeconds = jObj.GetInt(RelyingPartyParameters.AssertionExpirationTimeInSeconds),
                ClaimMappings = mappings
            };
        }
    }
}
