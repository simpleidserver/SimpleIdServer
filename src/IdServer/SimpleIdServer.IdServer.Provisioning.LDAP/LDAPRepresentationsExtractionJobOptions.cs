// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Configuration;

namespace SimpleIdServer.IdServer.Provisioning.LDAP;

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

    #region Users

    [ConfigurationRecord("Users DN", "Full DN of LDAP tree where your users are.", order: 4)]
    public string UsersDN { get; set; }
    [ConfigurationRecord("User object classes", "All values of LDAP objectClass attribute for users in LDAP, divided by commas.", order: 5)]
    public string UserObjectClasses { get; set; } = "person,organizationalPerson";

    #endregion

    #region Groups

    [ConfigurationRecord("Groups DN", "Full DN of LDAP tree where your groups are.", order: 6)]
    public string GroupsDN { get; set; }
    [ConfigurationRecord("Group object classes", "All values of LDAP objectClass attribute for groups in LDAP, divided by commas.", order: 7)]
    public string GroupObjectClasses { get; set; } = "posixGroup";
    [ConfigurationRecord("Membership Group LDAP Attribute.", "It is the name of the LDAP Attribute on the group, which is used for membership mappings, for example memberUid", order: 8)]
    public string MembershipLDAPAttribute { get; set; }
    [ConfigurationRecord("Membership User LDAP Attribute.", "It is the name of the LDAP Attribute on the user, which is used for membership mappings, for example uidNumber", order: 9)]
    public string MembershipUserLDAPAttribute { get; set; }
    [ConfigurationRecord("User Groups Retrieve Strategy", "Membership User LDAP Attribute.", order: 10)]
    public LoadingStrategies RetrievingStrategies { get; set; }
    [ConfigurationRecord("Member of LDAP Attribute", "Specifies the name of the LDAP Attribute on the LDAP user which contains the groups, which the user is member of.", order: 11, "RetrievingStrategies=LOAD_FROM_USER_MEMBEROF_ATTRIBUTE")]
    public string MemberOfAttribute { get; set; }

    #endregion

    [ConfigurationRecord("UUID LDAP Attribute", "Name of the LDAP attribute, which is used as a unique object identifier (UUID) for objects in LDAP, objectSID for active directory", order: 12)]
    public string UUIDLDAPAttribute { get; set; } = "entryUUID";
    [ConfigurationRecord("Modification Date Attribute", "Name of the LDAP Attribute, which is used as the modification date for objects in LDAP", order: 13)]
    public string ModificationDateAttribute { get; set; } = "modificationDate";
    [ConfigurationRecord("Batch size", "Number of records", order: 14)]
    public int BatchSize { get; set; } = 1;
}

public enum LoadingStrategies
{
    [ConfigurationRecordEnum("LOAD_BY_MEMBER_ATTRIBUTE")]
    LOAD_BY_MEMBER_ATTRIBUTE = 0,
    [ConfigurationRecordEnum("LOAD_FROM_USER_MEMBEROF_ATTRIBUTE")]
    LOAD_FROM_USER_MEMBEROF_ATTRIBUTE = 1
}
