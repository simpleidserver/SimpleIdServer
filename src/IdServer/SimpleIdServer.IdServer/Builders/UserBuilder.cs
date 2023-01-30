// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using System;
using System.Collections.Generic;
using static SimpleIdServer.IdServer.Constants;

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

        #region Claims

        public UserBuilder SetEmail(string email)
        {
            _user.UpdateClaim(JwtRegisteredClaimNames.Email, email);
            return this;
        }

        public UserBuilder SetPicture(string picture)
        {
            _user.UpdateClaim(UserClaims.Picture, picture);
            return this;
        }

        public UserBuilder AddRole(string role)
        {
            _user.AddClaim(UserClaims.Role, role);
            return this;
        }

        #endregion

        #region Consents

        public UserBuilder AddConsent(string clientId, params string[] scopes)
        {
            _user.Consents.Add(new Consent { Id = Guid.NewGuid().ToString(), ClientId = clientId, Scopes = scopes });
            return this;
        }

        public UserBuilder AddConsent(string clientId, IEnumerable<string> scopes, IEnumerable<string> claims)
        {
            _user.Consents.Add(new Consent { Id = Guid.NewGuid().ToString(), Scopes = scopes, ClientId = clientId, Claims = claims });
            return this;
        }

        #endregion

        public UserBuilder AddSession(string id, DateTime expirationTime)
        {
            _user.Sessions.Add(new UserSession { SessionId = id, AuthenticationDateTime = DateTime.UtcNow, ExpirationDateTime = expirationTime, State = UserSessionStates.Active });
            return this;
        }

        public UserBuilder GenerateRandomOTPKey()
        {
            var key = KeyGeneration.GenerateRandomKey(20);
            _user.OTPKey = key.ConvertFromBase32();
            return this;
        }

        public User Build() => _user;
    }
}
