// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Jobs;
using System;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;

namespace SimpleIdServer.IdServer.UI.Services
{
    public class LDAPAuthenticationService : IIdProviderAuthService
    {
        private readonly ILogger<LDAPAuthenticationService> _logger;

        public LDAPAuthenticationService(ILogger<LDAPAuthenticationService> logger)
        {
            _logger = logger;
        }

        public string Name => LDAPRepresentationsExtractionJob.NAME;

        public bool Authenticate(User user, IdentityProvisioning identityProvisioning, string password)
        {
            var distinguishedName = user.OAuthUserClaims.Single(c => c.Name == Constants.LDAPDistinguishedName).Value;
            var options = Serializer.PropertiesSerializer.DeserializeOptions<LDAPRepresentationsExtractionJobOptions, IdentityProvisioningProperty>(identityProvisioning.Properties);
            var credentials = new NetworkCredential(distinguishedName, password);
            using(var ldapConnection = new LdapConnection(new LdapDirectoryIdentifier(options.Server, options.Port), credentials, AuthType.Basic))
            {
                try
                {
                    ldapConnection.SessionOptions.ProtocolVersion = 3;
                    ldapConnection.Bind();
                    return true;
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    return false;
                }
            }
        }
    }
}
