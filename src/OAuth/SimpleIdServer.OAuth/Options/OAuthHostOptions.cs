// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt;
using SimpleIdServer.OAuth.Api.Token.TokenProfiles;
using SimpleIdServer.OAuth.Domains.Clients;
using SimpleIdServer.OAuth.Domains.Scopes;
using SimpleIdServer.OAuth.Domains.Users;
using System.Collections.Generic;

namespace SimpleIdServer.OAuth.Options
{
    public class OAuthHostOptions
    {
        public OAuthHostOptions()
        {
            DefaultScopes = new List<string>();
            DefaultTokenProfile = BearerTokenProfile.DEFAULT_NAME;
            ClientSecretExpirationInSeconds = null;
            SoftwareStatementTrustedParties = new List<SoftwareStatementTrustedParty>();
            DefaultOAuthClients = new List<OAuthClient>();
            DefaultJsonWebKeys = new List<JsonWebKey>();
            DefaultUsers = new List<OAuthUser>();
            DefaultOAuthScopes = new List<OAuthScope>();
            DefaultCulture = "en";
        }

        /// <summary>
        /// OAUTH2.0 client's default scopes.
        /// </summary>
        public ICollection<string> DefaultScopes { get; set; }
        /// <summary>
        /// OAUTH2.0 client's default token profile.
        /// </summary>
        public string DefaultTokenProfile { get; set; }
        /// <summary>
        /// OAUTH2.0 default clients.
        /// </summary>
        public List<OAuthClient> DefaultOAuthClients { get; set; }
        /// <summary>
        /// OAUTH2.0 default json web keys.
        /// </summary>
        public List<JsonWebKey> DefaultJsonWebKeys { get; set; }
        /// <summary>
        /// OAUTH2.0 default users.
        /// </summary>
        public List<OAuthUser> DefaultUsers { get; set; }
        /// <summary>
        /// OAUTH2.0 default scopes.
        /// </summary>
        public List<OAuthScope> DefaultOAuthScopes { get; set; }
        /// <summary>
        /// Client secret expiration time in seconds.
        /// </summary>
        public int? ClientSecretExpirationInSeconds { get; set; }
        /// <summary>
        /// Trusted parties used to validate the software statement.
        /// </summary>
        public ICollection<SoftwareStatementTrustedParty> SoftwareStatementTrustedParties { get; set; }
        /// <summary>
        /// Set the default UI cutlure.
        /// </summary>
        public string DefaultCulture { get; set; }
    }
}