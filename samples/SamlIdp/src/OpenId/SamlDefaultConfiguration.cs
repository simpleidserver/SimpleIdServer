// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Common.Domains;
using SimpleIdServer.Common.Helpers;
using SimpleIdServer.Saml.Idp.Domains;
using System;
using System.Collections.Generic;

namespace OpenId
{
    public class SamlDefaultConfiguration
    {
        public static ICollection<RelyingPartyAggregate> RelyingParties = new List<RelyingPartyAggregate>
        {
            new RelyingPartyAggregate
            {
                Id = "urn:sp",
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                MetadataUrl = "http://localhost:7001/saml/metadata",
                AssertionExpirationTimeInSeconds = 60,
                ClaimMappings = new List<RelyingPartyClaimMapping>
                {
                    new RelyingPartyClaimMapping
                    {
                        UserAttribute = SimpleIdServer.Jwt.Constants.UserClaims.GivenName,
                        ClaimName = "urn:oid:2.5.4.42",
                        ClaimFormat = ""
                    },
                    new RelyingPartyClaimMapping
                    {
                        UserAttribute= SimpleIdServer.Jwt.Constants.UserClaims.FamilyName,
                        ClaimName = "LastName",
                        ClaimFormat = ""
                    }
                }
            },
            new RelyingPartyAggregate
            {
                Id = "urn:idp",
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                MetadataUrl = "https://localhost:60000/saml/metadata",
                AssertionExpirationTimeInSeconds = 60,
                ClaimMappings = new List<RelyingPartyClaimMapping>
                {
                    new RelyingPartyClaimMapping
                    {
                        UserAttribute = SimpleIdServer.Jwt.Constants.UserClaims.GivenName,
                        ClaimName = "urn:oid:2.5.4.42",
                        ClaimFormat = ""
                    },
                    new RelyingPartyClaimMapping
                    {
                        UserAttribute= SimpleIdServer.Jwt.Constants.UserClaims.FamilyName,
                        ClaimName = "LastName",
                        ClaimFormat = ""
                    }
                }
            }
        };

        public static ICollection<User> Users = new List<User>
        {
            new User
            {
                Id = "sub",
                Credentials = new List<UserCredential>
                {
                    new UserCredential
                    {
                        CredentialType = "samlpwd",
                        Value = PasswordHelper.ComputeHash("password")
                    }
                },
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                OTPKey = "HQI6X6V2MEP44J4NLZJ65VKAHCSSCNFL",
                DeviceRegistrationToken = "ciyortoPQHGluxo-vIZLu7:APA91bHRrB-mdgHl6IQFu4XNWR5VBXxOjaq-gAAuxCzswQAGeryvFaBqoJqJN_oSEtPZMTknRe2rixJj5cjnaWkCin8NSXm7Gug6peZd9EpJgJ98CNHqOudcFv_h3jp4dpgWn6imb7sR",
                OAuthUserClaims = new List<UserClaim>
                {
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.Subject, "sub"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.Name, "name"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.FamilyName, "familyName"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.UniqueName, "uniquename"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.GivenName, "givenName"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.MiddleName, "middleName"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.NickName, "nickName"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.BirthDate, "07-10-1989"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.PreferredUserName, "preferredUserName"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.ZoneInfo, "zoneInfo"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.Locale, "locale"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.Picture, "picture"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.WebSite, "website"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.Profile, "profile"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.Gender, "gender"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.Email, "agentsimpleidserver@gmail.com"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.UpdatedAt, "1612355959", SimpleIdServer.Jwt.ClaimValueTypes.INTEGER),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.EmailVerified, "true", SimpleIdServer.Jwt.ClaimValueTypes.BOOLEAN),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.Address, "{ 'street_address': '1234 Hollywood Blvd.', 'locality': 'Los Angeles', 'region': 'CA', 'postal_code': '90210', 'country': 'US' }", SimpleIdServer.Jwt.ClaimValueTypes.JSONOBJECT),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.PhoneNumber, "+1 (310) 123-4567"),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.PhoneNumberVerified, "true", SimpleIdServer.Jwt.ClaimValueTypes.BOOLEAN),
                    new UserClaim(SimpleIdServer.Jwt.Constants.UserClaims.Role, "visitor")
                }
            }
        };
    }
}
