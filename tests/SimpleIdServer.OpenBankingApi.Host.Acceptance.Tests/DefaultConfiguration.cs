// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OpenID.Domains;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace SimpleIdServer.OpenBankingApi.Host.Acceptance.Tests
{
    public class DefaultConfiguration
    {
        public static List<OAuthUser> Users => new List<OAuthUser>
        {
            new OAuthUser
            {
                Id = "administrator",
                Credentials = new List<OAuthUserCredential>
                {
                    new OAuthUserCredential
                    {
                        CredentialType = "pwd",
                        Value = PasswordHelper.ComputeHash("password")
                    }
                },
                AuthenticationTime = DateTime.UtcNow,
                Claims = new List<Claim>
                {
                    new Claim(Jwt.Constants.UserClaims.Subject, "administrator"),
                    new Claim(Jwt.Constants.UserClaims.Name, "Thierry Habart"),
                    new Claim(Jwt.Constants.UserClaims.Email, "habarthierry@hotmail.fr"),
                    new Claim(Jwt.Constants.UserClaims.Role, "role1"),
                    new Claim(Jwt.Constants.UserClaims.Role, "role2"),
                    new Claim(Jwt.Constants.UserClaims.UpdatedAt, "1612361922", Jwt.ClaimValueTypes.INTEGER),
                    new Claim(Jwt.Constants.UserClaims.EmailVerified, "true", Jwt.ClaimValueTypes.BOOLEAN),
                    new Claim(Jwt.Constants.UserClaims.Address, "{ 'street_address': '1234 Hollywood Blvd.', 'region': 'CA' }", Jwt.ClaimValueTypes.JSONOBJECT)
                }
            }
        };

        public static List<OpenIdScope> Scopes => new List<OpenIdScope>
        {
            new OpenIdScope
            {
                Name = "accounts",
                IsExposedInConfigurationEdp = true
            }
        };
    }
}