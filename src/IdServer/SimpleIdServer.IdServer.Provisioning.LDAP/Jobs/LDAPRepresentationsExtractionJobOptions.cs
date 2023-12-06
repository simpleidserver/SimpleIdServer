// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Configuration;

namespace SimpleIdServer.IdServer.Provisioning.LDAP.Jobs;

public class LDAPRepresentationsExtractionJobOptions
{
    [ConfigurationRecord("Server", null, order: 0)]
    public string Server { get; set; }
    [ConfigurationRecord("Port", null, order: 1)]
    public int Port { get; set; } = 389;
    [ConfigurationRecord("Bind DN", "DN of the LDAP admin, which will be used by IdServer to access LDAP Server", order: 2)]
    public string BindDN { get; set; }
    [ConfigurationRecord("Bind Credentials", "Password of LDAP admin.", 3, null, CustomConfigurationRecordType.PASSWORD)]
    public string BindCredentials { get; set; }
    [ConfigurationRecord("Users DN", "Full DN of LDAP tree where your users are.", order: 4)]
    public string UsersDN { get; set; }
    [ConfigurationRecord("User object classes", "All values of LDAP objectClass attribute for users in LDAP, divided by commas.", order: 5)]
    public string UserObjectClasses { get; set; } = "person,organizationalPerson";
    [ConfigurationRecord("UUID LDAP Attribute", "Name of the LDAP attribute, which is used as a unique object identifier (UUID) for objects in LDAP.", order: 6)]
    public string UUIDLDAPAttribute { get; set; } = "entryUUID";
    [ConfigurationRecord("Modification Date Attribute", "Name of the LDAP Attribute, which is used as the modification date for objects in LDAP", order: 7)]
    public string ModificationDateAttribute { get; set; } = "modificationDate";
    [ConfigurationRecord("Batch size", "Number of records", order: 8)]
    public int BatchSize { get; set; } = 1;
}
