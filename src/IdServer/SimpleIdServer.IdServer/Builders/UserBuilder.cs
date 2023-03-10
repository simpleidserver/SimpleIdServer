// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Domains.DTOs;
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

        public static UserBuilder Create(string login, string password, string name = null, Realm realm = null)
        {
            var result = new UserBuilder();
            result._user.Id = Guid.NewGuid().ToString();
            result._user.Name = login;
            result._user.Credentials.Add(new UserCredential
            {
                Id = Guid.NewGuid().ToString(),
                CredentialType = "pwd",
                IsActive = true,
                Value = PasswordHelper.ComputeHash(password)
            });
            if (realm == null) result._user.Realms.Add(Constants.StandardRealms.Master);
            else result._user.Realms.Add(realm);
            result._user.CreateDateTime = DateTime.UtcNow;
            result._user.UpdateDateTime = DateTime.UtcNow;
            return result;
        }

        #region User claims

        public UserBuilder SetFirstname(string firstName)
        {
            _user.Firstname = firstName;
            return this;
        }

        public UserBuilder SetLastname(string lastName)
        {
            _user.Lastname = lastName;
            return this;
        }

        public UserBuilder SetEmail(string email)
        {
            _user.Email = email;
            return this;
        }

        public UserBuilder SetEmailVerified(bool emailVerified)
        {
            _user.EmailVerified = emailVerified;
            return this;
        }

        #endregion

        #region Claims

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

        public UserBuilder SetAddress(string street, string locality, string region, string postalCode, string country, string formatted)
        {
            _user.UpdateClaim(AddressClaimNames.Street, street);
            _user.UpdateClaim(AddressClaimNames.Locality, locality);
            _user.UpdateClaim(AddressClaimNames.Region, region);
            _user.UpdateClaim(AddressClaimNames.PostalCode, postalCode);
            _user.UpdateClaim(AddressClaimNames.Country, country);
            _user.UpdateClaim(AddressClaimNames.Formatted, formatted);
            return this;
        }

        #endregion

        #region Consents

        public UserBuilder AddConsent(string clientId, params string[] scopes)
        {
            _user.Consents.Add(new Consent { Id = Guid.NewGuid().ToString(), ClientId = clientId, Scopes = scopes, CreateDateTime = DateTime.UtcNow });
            return this;
        }

        public UserBuilder AddConsent(string clientId, IEnumerable<string> scopes, IEnumerable<string> claims)
        {
            _user.Consents.Add(new Consent { Id = Guid.NewGuid().ToString(), Scopes = scopes, ClientId = clientId, Claims = claims, CreateDateTime = DateTime.UtcNow });
            return this;
        }

        #endregion

        public UserBuilder AddSession(string id, DateTime expirationTime)
        {
            _user.Sessions.Add(new UserSession { SessionId = id, AuthenticationDateTime = DateTime.UtcNow, ExpirationDateTime = expirationTime, State = UserSessionStates.Active });
            return this;
        }

        public UserBuilder GenerateRandomHOTPKey()
        {
            _user.GenerateHOTP();
            return this;
        }

        public UserBuilder GenerateRandomTOTPKey()
        {
            _user.GenerateTOTP();
            return this;
        }

        public User Build() => _user;
    }
}
