// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Serializer;

namespace SimpleIdServer.IdServer.Jobs
{
    public class LDAPRepresentationsExtractionJobOptions
    {
        [VisibleProperty("Server")]
        public string Server { get; set; }
        [VisibleProperty("Port")]
        public int Port { get; set; } = 389;
        [VisibleProperty("Bind DN", "DN of the LDAP admin, which will be used by IdServer to access LDAP Server")]
        public string BindDN { get; set; }
        [VisibleProperty("Bind Credentials", "Password of LDAP admin.")]
        public string BindCredentials { get; set; }
        [VisibleProperty("Users DN", "Full DN of LDAP tree where your users are.")]
        public string UsersDN { get; set; }
        [VisibleProperty("User object classes", "All values of LDAP objectClass attribute for users in LDAP, divided by commas.")]
        public string UserObjectClasses { get; set; } = "person,organizationalPerson";
        [VisibleProperty("UUID LDAP Attribute", "Name of the LDAP attribute, which is used as a unique object identifier (UUID) for objects in LDAP.")]
        public string UUIDLDAPAttribute { get; set; } = "entryUUID";
        [VisibleProperty("Batch size", "Number of records")]
        public int BatchSize { get; set; } = 1;
    }
}
