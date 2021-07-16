// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OpenBankingApi.Domains.Account;
using SimpleIdServer.OpenBankingApi.Domains.Account.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Claims;

namespace SimpleIdServer.OpenBankingApi.Host.Acceptance.Tests
{
    public class DefaultConfiguration
    {
        public static ConcurrentBag<AccountAggregate> Accounts = new ConcurrentBag<AccountAggregate>
        {
            new AccountAggregate
            {
                AggregateId = "22289",
                Subject = "sub",
                Status = AccountStatus.Enabled,
                StatusUpdateDateTime = DateTime.UtcNow,
                AccountSubType = AccountSubTypes.CurrentAccount,
                Accounts = new List<CashAccount>
                {
                    new CashAccount
                    {
                        Identification = "80200110203345",
                        SecondaryIdentification = "00021"
                    }
                }
            },
            new AccountAggregate
            {
                AggregateId = "31820",
                Subject = "sub",
                Status = AccountStatus.Enabled,
                StatusUpdateDateTime = DateTime.UtcNow,
                AccountSubType = AccountSubTypes.Savings,
                Accounts = new List<CashAccount>
                {
                    new CashAccount
                    {
                        Identification = "10-159-60",
                        SecondaryIdentification = "789012345"
                    }
                }
            }
        };

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
                Sessions = new List<OAuthUserSession>
                {
                    new OAuthUserSession
                    {
                        AuthenticationDateTime = DateTime.UtcNow,
                        ExpirationDateTime = DateTime.UtcNow.AddSeconds(5 * 60),
                        SessionId = Guid.NewGuid().ToString()
                    }
                },
                OAuthUserClaims = new List<OAuthUserClaim>
                {
                    new OAuthUserClaim(Jwt.Constants.UserClaims.Subject, "administrator"),
                    new OAuthUserClaim(Jwt.Constants.UserClaims.Name, "Thierry Habart"),
                    new OAuthUserClaim(Jwt.Constants.UserClaims.Email, "habarthierry@hotmail.fr"),
                    new OAuthUserClaim(Jwt.Constants.UserClaims.Role, "role1"),
                    new OAuthUserClaim(Jwt.Constants.UserClaims.Role, "role2"),
                    new OAuthUserClaim(Jwt.Constants.UserClaims.UpdatedAt, "1612361922", Jwt.ClaimValueTypes.INTEGER),
                    new OAuthUserClaim(Jwt.Constants.UserClaims.EmailVerified, "true", Jwt.ClaimValueTypes.BOOLEAN),
                    new OAuthUserClaim(Jwt.Constants.UserClaims.Address, "{ 'street_address': '1234 Hollywood Blvd.', 'region': 'CA' }", Jwt.ClaimValueTypes.JSONOBJECT)
                }
            }
        };

        public static List<OAuthScope> Scopes => new List<OAuthScope>
        {
            new OAuthScope
            {
                Name = "accounts",
                IsExposedInConfigurationEdp = true
            }
        };
    }
}