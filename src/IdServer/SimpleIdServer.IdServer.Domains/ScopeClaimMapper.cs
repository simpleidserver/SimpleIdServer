// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    public class ScopeClaimMapper : IClaimMappingRule
    {
        public ScopeClaimMapper() { }

        [JsonPropertyName(ScopeClaimMapperNames.Id)]
        public string Id { get; set; } = null!;

        [JsonPropertyName(ScopeClaimMapperNames.Name)]
        public string Name { get; set; } = null!;
        [JsonPropertyName(ScopeClaimMapperNames.MapperType)]
        public MappingRuleTypes MapperType { get; set; }
        /// <summary>
        /// User's attribute name.
        /// </summary>
        [JsonPropertyName(ScopeClaimMapperNames.SourceUserAttribute)]
        public string? SourceUserAttribute { get; set; } = null;
        /// <summary>
        /// User's property name.
        /// </summary>
        [JsonPropertyName(ScopeClaimMapperNames.SourceUserProperty)]
        public string? SourceUserProperty { get; set; } = null;
        [JsonPropertyName(ScopeClaimMapperNames.SourceScimPath)]
        public string? SourceScimPath { get; set; } = null;
        /// <summary>
        /// Name of the claim to insert in the token.
        /// </summary>
        [JsonPropertyName(ScopeClaimMapperNames.TargetClaimPath)]
        public string? TargetClaimPath { get; set; } = null;
        /// <summary>
        /// Include the claims into the access token.
        /// </summary>
        [JsonPropertyName(ScopeClaimMapperNames.IncludeInAccessToken)]
        public bool IncludeInAccessToken { get; set; } = false;
        /// <summary>
        /// SAML Attribute name.
        /// </summary>
        [JsonPropertyName(ScopeClaimMapperNames.SAMLAttributeName)]
        public string? SAMLAttributeName { get; set; } = null;
        /// <summary>
        /// JSON type of the claim.
        /// </summary>
        [JsonPropertyName(ScopeClaimMapperNames.TokenClaimJsonType)]
        public TokenClaimJsonTypes? TokenClaimJsonType { get; set; } = null;
        [JsonPropertyName(ScopeClaimMapperNames.IsMultivalued)]
        public bool IsMultiValued { get; set; } = false;
        [JsonIgnore]
        public Scope Scope { get; set; }

        public static ScopeClaimMapper CreateOpenIdAttributeClaim(string name, string tokenClaimName, string userAttributeName, TokenClaimJsonTypes claimJsonType = TokenClaimJsonTypes.STRING)
        {
            return new ScopeClaimMapper
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                MapperType = MappingRuleTypes.USERATTRIBUTE,
                SourceUserAttribute = userAttributeName,
                TargetClaimPath = tokenClaimName,
                TokenClaimJsonType = claimJsonType
            };
        }

        public static List<ScopeClaimMapper> CreateOpenIdAddressClaim()
        {
            return CreateOpenIdAddressClaim(AddressClaimNames.Street, AddressClaimNames.Locality, AddressClaimNames.Region, AddressClaimNames.PostalCode, AddressClaimNames.Country, AddressClaimNames.Formatted);
        }

        public static List<ScopeClaimMapper> CreateOpenIdAddressClaim(string street, string locality, string region, string postalCode, string country, string formatted)
        {
            return new List<ScopeClaimMapper>
            {
                new ScopeClaimMapper
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Street",
                    MapperType = MappingRuleTypes.USERATTRIBUTE,
                    SourceUserAttribute = AddressClaimNames.Street,
                    TargetClaimPath = $"address.{AddressClaimNames.Street}",
                    TokenClaimJsonType = TokenClaimJsonTypes.STRING
                },
                new ScopeClaimMapper
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Locality",
                    MapperType = MappingRuleTypes.USERATTRIBUTE,
                    SourceUserAttribute = AddressClaimNames.Locality,
                    TargetClaimPath = $"address.{AddressClaimNames.Locality}",
                    TokenClaimJsonType = TokenClaimJsonTypes.STRING
                },
                new ScopeClaimMapper
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Region",
                    MapperType = MappingRuleTypes.USERATTRIBUTE,
                    SourceUserAttribute = AddressClaimNames.Region,
                    TargetClaimPath = $"address.{AddressClaimNames.Region}",
                    TokenClaimJsonType = TokenClaimJsonTypes.STRING
                },
                new ScopeClaimMapper
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Postal code",
                    MapperType = MappingRuleTypes.USERATTRIBUTE,
                    SourceUserAttribute = AddressClaimNames.PostalCode,
                    TargetClaimPath = $"address.{AddressClaimNames.PostalCode}",
                    TokenClaimJsonType = TokenClaimJsonTypes.STRING
                },
                new ScopeClaimMapper
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Country",
                    MapperType = MappingRuleTypes.USERATTRIBUTE,
                    SourceUserAttribute = AddressClaimNames.Country,
                    TargetClaimPath = $"address.{AddressClaimNames.Country}",
                    TokenClaimJsonType = TokenClaimJsonTypes.STRING
                },
                new ScopeClaimMapper
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Formatted",
                    MapperType = MappingRuleTypes.USERATTRIBUTE,
                    SourceUserAttribute = AddressClaimNames.Formatted,
                    TargetClaimPath = $"address.{AddressClaimNames.Formatted}",
                    TokenClaimJsonType = TokenClaimJsonTypes.STRING
                },
            };
        }

        public static ScopeClaimMapper CreateOpenIdAttributeClaimArray(string name, string tokenClaimName, string userAttributeName, TokenClaimJsonTypes claimJsonType = TokenClaimJsonTypes.STRING)
        {
            return new ScopeClaimMapper
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                MapperType = MappingRuleTypes.USERATTRIBUTE,
                SourceUserAttribute = userAttributeName,
                TargetClaimPath = tokenClaimName,
                TokenClaimJsonType = claimJsonType,
                IsMultiValued = true
            };
        }

        public static ScopeClaimMapper CreateOpenIdPropertyClaim(string name, string tokenClaimName, string userPropertyName, TokenClaimJsonTypes claimJsonType = TokenClaimJsonTypes.STRING)
        {
            return new ScopeClaimMapper
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                MapperType = MappingRuleTypes.USERPROPERTY,
                SourceUserProperty = userPropertyName,
                TargetClaimPath = tokenClaimName,
                TokenClaimJsonType = claimJsonType
            };
        }

        public static ScopeClaimMapper CreateOpenIdSubjectClaim()
        {
            return new ScopeClaimMapper
            {
                Id = Guid.NewGuid().ToString(),
                Name = "sub",
                TargetClaimPath = "sub",
                MapperType = MappingRuleTypes.SUBJECT,
            };
        }

        public static ScopeClaimMapper CreateSAMLNameIdentifierClaim()
        {
            return new ScopeClaimMapper
            {
                Id = Guid.NewGuid().ToString(),
                Name = "nameidentifier",
                TargetClaimPath = "sub",
                MapperType = MappingRuleTypes.SUBJECT,
                SAMLAttributeName = ClaimTypes.NameIdentifier
            };
        }

        public static ScopeClaimMapper CreateSAMLPropertyClaim(string name, string samlClaimName, string userPropertyName, TokenClaimJsonTypes claimJsonType = TokenClaimJsonTypes.STRING)
        {
            return new ScopeClaimMapper
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                MapperType = MappingRuleTypes.USERPROPERTY,
                SourceUserProperty = userPropertyName,
                SAMLAttributeName = samlClaimName,
                TokenClaimJsonType = claimJsonType
            };
        }

        public static ScopeClaimMapper CreateSamlPropertyClaimFromScim(string name, string samlClaimName, string sourceScimPath, TokenClaimJsonTypes claimJsonType = TokenClaimJsonTypes.STRING)
        {
            return new ScopeClaimMapper
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                MapperType = MappingRuleTypes.SCIM,
                SAMLAttributeName = samlClaimName,
                SourceScimPath = sourceScimPath,
                TokenClaimJsonType = claimJsonType
            };
        }
    }
}
