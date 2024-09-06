# Automatic

import DocsCards from '@site/src/components/global/DocsCards';
import DocsCard from '@site/src/components/global/DocsCard';

The [Automatic Identity Provisioning](../glossary) workflow in SimpleIdServer can be configured to extract users from various types of storage and create their accounts in the local [Identity Provider](../glossary).

The workflow consists of two steps:

1. **Extract** : Retrieve users from the storage and store the results in CSV files. Only new or modified users are extracted.
2. **Import** : Read CSV files and create or update the local accounts.

There are two supported types of storage.

<DocsCards>
    <DocsCard header="LDAP" href="#ldap">
    </DocsCard>
    <DocsCard header="SCIM" href="#scim">
    </DocsCard>
</DocsCards>

## LDAP

To configure automatic identity provisioning with LDAP, follow these steps :

1. Navigate to the `Identity Provisioning` menu and click on the `Automatic Identity Provisioning` button.
2. Select `LDAP`.
3. Navigate to the `Properties` tab.
4. Update the properties with the appropriate values

| Key                          | Description                                                                                        | Default value               |
| ---------------------------- | -------------------------------------------------------------------------------------------------- | --------------------------- |
| Server                       | DNS of your server                                                                                 | localhost                   |
| Port                         | LDAP Port                                                                                          | 389                         |
| Bind DN                      | DN of the LDAP Admin                                                                               | cn=admin,dc=xl,dc=com       |
| Bind Credentials             | Password of the LDAP Admin                                                                         | password                    |
| Users DN                     | Full DN of LDAP tree where you users are                                                           | ou=people,dc=xl,dc=com      |
| User object class            | All values of LDAP objectClass attribute for users in LDAP, divided by commas                      | organizationalPerson,person |
| entryUUID                    | Name of the LDAP Attribute, which is used as a unique object identifier (UUID) for objects in LDAP | entryUUID                   |
| Modification Date Attribute  | Name of the LDAP Attribute, which is used as the modification date for objects in LDAP             | modificationDate            |
| Batch size                   | Number of records stored in each CSV file                                                          | 1                           |

5. Click on the `Update` button.
6. Click on the `Try to extract the users` button to check your settings.

Once everything is configured, you can edit the Mapping rules, which are used during extraction to transform LDAP Attributes into [User Attribute](../glossary) or [User Property](../glossary).

The `Add mapping rule` popup displays all the attributes coming from LDAP to facilitate the editing of the mapping rules.

## SCIM

To configure identity provisioning with SCIM, follow these steps:

1. Navigate to the `Identity Provisioning` menu and click on the `Automatic Identity Provisioning` button.
2. Select `SCIM`.
3. Navigate to the `Properties` tab.
4. Update the properties with the appropriate values.

| Key                  | Description                                                                                          | Default value                        |
| -------------------- | ---------------------------------------------------------------------------------------------------- | ------------------------------------ |
| SCIM Endpoint        | Base URL of the SCIM endpoint.                                                                       | https://localhost:5003               |
| Authentication Types | Authentication type used to access the SCIM API.                                                     | API Key                              |
| API Key              | Required when the authentication type is equal to **API Key**. This value is passed to the SCIM API. | ba521b3b-02f7-4a37-b03c-58f713bf88e7 |
| Client Id            | Required when the authentication type is equals to ¨**Client Secret Post**.                          |                                      |
| Client Secret        | Required when the authentication type is equals to **Client Secret Post**.                           |                                      |
| Count                | Number of records stored in each CSV file.                                                           | 1                                    |

There are two authentication types, depending on the security of your SCIM API, you must choose the correct one:

* **API Key** : By default, our [SCIM API is protected by an API Key](../scim2), and the workflow uses the API Key to access the SCIM API.
* **Client secret post** : The workflow uses the Client Credentials grant to obtain an access token and uses it to access the SCIM API.

5. Click on the `Update` button.
6. Click on the `Try to extract the users` button to check your settings.

Once everything is configured, you can edit the Mapping rules, which are used during extraction to transform LDAP Attributes into [User Attribute](../glossary) or [User Property](../glossary).

The `Add mapping rule` popup displays all the attributes coming from SCIM to facilitate the editing of the mapping rules.