// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;

namespace SimpleIdServer.IdServer.Builders
{
    public class UserBuilder
    {
        private readonly User _user;

        private UserBuilder() 
        { 
            _user = new User();
        }

        public static UserBuilder Create(string login, string password, string name = null)
        {
            var result = new UserBuilder();
            result._user.Id = login;
            result._user.Credentials.Add(new UserCredential
            {
                CredentialType = "pwd",
                Value = PasswordHelper.ComputeHash(password)
            });
            result._user.OAuthUserClaims.Add(new UserClaim { Value = login, Name = JwtRegisteredClaimNames.Sub });
            if (!string.IsNullOrWhiteSpace(name))
                result._user.OAuthUserClaims.Add(new UserClaim { Value = name, Name = JwtRegisteredClaimNames.Name });

            return result;
        }

        public User Build() => _user;
    }
}
