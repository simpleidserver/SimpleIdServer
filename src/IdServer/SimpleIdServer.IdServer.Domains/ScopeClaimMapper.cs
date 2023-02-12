// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains.DTOs;
using System.Security.Claims;

namespace SimpleIdServer.IdServer.Domains
{
    public class ScopeClaimMapper
    {
        public ScopeClaimMapper() { }

        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public ScopeClaimMapperTypes MapperType { get; set; }
        /// <summary>
        /// User's attribute name.
        /// </summary>
        public string? UserAttributeName { get; set; } = null;
        /// <summary>
        /// User's attribute name for the street.
        /// </summary>
        public string? UserAttributeStreetName { get; set; } = null;
        /// <summary>
        /// User's attribute name for the locality.
        /// </summary>
        public string? UserAttributeLocalityName { get; set; } = null;
        /// <summary>
        /// User's attribute name for the region.
        /// </summary>
        public string? UserAttributeRegionName { get; set; } = null;
        /// <summary>
        /// User's attribute name for the postal code.
        /// </summary>
        public string? UserAttributePostalCodeName { get; set; } = null;
        /// <summary>
        /// User's attribute name for the country.
        /// </summary>
        public string? UserAttributeCountryName { get; set; } = null;
        /// <summary>
        /// User's attribute name for the formatted address..
        /// </summary>
        public string? UserAttributeFormattedName { get; set; } = null;
        /// <summary>
        /// User's property name.
        /// </summary>
        public string? UserPropertyName { get; set; } = null;
        /// <summary>
        /// Name of the claim to insert in the token.
        /// </summary>
        public string? TokenClaimName { get; set; } = null;
        /// <summary>
        /// SAML Attribute name.
        /// </summary>
        public string? SAMLAttributeName { get; set; } = null;
        /// <summary>
        /// JSON type of the claim.
        /// </summary>
        public TokenClaimJsonTypes? TokenClaimJsonType { get; set; } = null;
        public bool IsMultiValued { get; set; } = false;

        public static ScopeClaimMapper CreateOpenIdAttributeClaim(string name, string tokenClaimName, string userAttributeName, TokenClaimJsonTypes claimJsonType = TokenClaimJsonTypes.STRING)
        {
            return new ScopeClaimMapper
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                MapperType = ScopeClaimMapperTypes.USERATTRIBUTE,
                UserAttributeName = userAttributeName,
                TokenClaimName = tokenClaimName,
                TokenClaimJsonType = claimJsonType
            };
        }

        public static ScopeClaimMapper CreateOpenIdAddressClaim(string name)
        {
            return new ScopeClaimMapper
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                MapperType = ScopeClaimMapperTypes.ADDRESS,
                TokenClaimName = name,
                TokenClaimJsonType = TokenClaimJsonTypes.JSON,
                UserAttributeStreetName = AddressClaimNames.Street,
                UserAttributeLocalityName = AddressClaimNames.Locality,
                UserAttributeRegionName = AddressClaimNames.Region,
                UserAttributePostalCodeName = AddressClaimNames.PostalCode,
                UserAttributeCountryName = AddressClaimNames.Country,
                UserAttributeFormattedName = AddressClaimNames.Formatted
            };
        }

        public static ScopeClaimMapper CreateOpenIdAttributeClaimArray(string name, string tokenClaimName, string userAttributeName, TokenClaimJsonTypes claimJsonType = TokenClaimJsonTypes.STRING)
        {
            return new ScopeClaimMapper
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                MapperType = ScopeClaimMapperTypes.USERATTRIBUTE,
                UserAttributeName = userAttributeName,
                TokenClaimName = tokenClaimName,
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
                MapperType = ScopeClaimMapperTypes.USERPROPERTY,
                UserPropertyName = userPropertyName,
                TokenClaimName = tokenClaimName,
                TokenClaimJsonType = claimJsonType
            };
        }

        public static ScopeClaimMapper CreateOpenIdSubjectClaim()
        {
            return new ScopeClaimMapper
            {
                Id = Guid.NewGuid().ToString(),
                Name = "sub",
                TokenClaimName = "sub",
                MapperType = ScopeClaimMapperTypes.SUBJECT,
            };
        }

        public static ScopeClaimMapper CreateSAMLNameIdentifierClaim()
        {
            return new ScopeClaimMapper
            {
                Id = Guid.NewGuid().ToString(),
                Name = "nameidentifier",
                TokenClaimName = "sub",
                MapperType = ScopeClaimMapperTypes.SUBJECT,
                SAMLAttributeName = ClaimTypes.NameIdentifier
            };
        }

        public static ScopeClaimMapper CreateSAMLPropertyClaim(string name, string samlClaimName, string userPropertyName, TokenClaimJsonTypes claimJsonType = TokenClaimJsonTypes.STRING)
        {
            return new ScopeClaimMapper
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                MapperType = ScopeClaimMapperTypes.USERPROPERTY,
                UserPropertyName = userPropertyName,
                SAMLAttributeName = samlClaimName,
                TokenClaimJsonType = claimJsonType
            };
        }
    }

    public enum ScopeClaimMapperTypes
    {
        USERATTRIBUTE = 0,
        USERPROPERTY = 1,
        ADDRESS = 2,
        SUBJECT = 3
    }

    public enum TokenClaimJsonTypes
    {
        STRING = 0,
        LONG = 1,
        INT = 2,
        BOOLEAN = 3,
        JSON = 4,
        DATETIME = 5
    }
}
