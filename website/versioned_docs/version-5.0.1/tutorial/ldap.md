
import Ldap1 from './images/ldap-1.png';
import Ldap2 from './images/ldap-2.png';
import Ldap3 from './images/ldap-3.png';

# LDAP - Identity Provisioning

Users stored in the LDAP repository can be imported into the Identity Server.

In this tutorial, we will explain how to import users from [OPENLDAP](https://www.openldap.org/) to the Identity Server.

## Install OPENLDAP

Before proceeding further, please ensure that Docker is installed on your machine.

Open a command prompt and launch an OPENLDAP instance by executing the following command line:

```
docker run -p 389:389 -p 636:636 --name ldap-service -h ldap-service -e LDAP_ORGANISATION="XL" -e LDAP_DOMAIN="xl.com" -e LDAP_ADMIN_PASSWORD="password" -d osixia/openldap:latest
```

This command exposes LDAP over port 389 and LDAPS over port 636.

## Create some users

Download and install [Apache Directory Studio](https://directory.apache.org/studio/downloads.html).

Authenticate as an admin with the appropriate login DN :

```
Login Credentials:
ID : cn=admin,dc=xl,dc=com
Password: password
```


Under `dc=xl,dc=com`, create an entry with the object class `Organizational Unit (organizationalUnit)`.
The `ou` attribute of this entry should be set to `people`. 
This entry will contain the users who will be migrated to the Identity Server.

<div style={{textAlign:"center"}}>
    <img src={Ldap1} style={{width: 400}} />
</div>


Under `ou=people`, add two entries, each containing two object classes: `Organizational Person (organizationalPerson)` and `person`.

The first user should have the following attributes and a password set to `password`.


| Attribute    | Value                |
| ------------ | -------------------- |
| objectClass  | organizationalPerson |
| objectClass  | person               |
| cn           | firstUser            |
| sn           | firstUser            |
| userPassword | password            |

<div style={{textAlign:"center"}}>
    <img src={Ldap2} style={{width: 400}} />
</div>

The second user should have the following attributes.

| Attribute   | Value                |
| ----------- | -------------------- |
| objectClass | organizationalPerson |
| objectClass | person               |
| cn          | secondUser           |
| sn          | secondUser           |

<div style={{textAlign:"center"}}>
    <img src={Ldap3} style={{width: 400}} />
</div>

Now that there is an up and running OPENLDAP server with two users, you can utilize the administration website to import both users.

## Extract

Browse the [administration UI](http://localhost:5002), navigate to the `Identity Provisioning` screen, and click on `LDAP`.


In the `Properties` tab, you can update the attributes of the extraction job

| Field                       | Description                                                                                         | Default value |
| --------------------------- | --------------------------------------------------------------------------------------------------- | ------------- |
| Server                      | -                                                                                                   | localhost     |
| Port                        | -                                                                                                   | 389           |
| Bind DN                     | DN of the LDAP admin, which will be used by IdServer to access LDAP Server                          | cn=admin,dc=xl,dc=com |
| Bind Credentials            | Password of LDAP admin                                                                              | password              |
| Users DN                    | Full DN of LDAP tree where users are                                                                | ou=people,dc=xl,dc=com |
| User object classes         | All values of LDAP objectClass attribute for users in LDAP, divided by commas                       | organizationalPerson,person |
| Groups DN                   | Full DN of LDAP tree where groups are                                                               | ou=groups,dc=xl,dc=com |
| Group object classes        | All values of LDAP objectClass attribute for groups in LDAP, divided by commas                      | posixGroup |
| Membership Group LDAP Attribute | It is the name of the LDAP Attribute on the group, which is used for membership mappings, for example memberUid | memberUid |
| Membership User LDAP Attribute | It is the name of the LDAP Attribute on the user, which is used for membership mappings, for example uidNumber | uidNumber |
| User Groups Retrieve Strategy | Membership User LDAP Attribute | LOAD_BY_MEMBER_ATTRIBUTE |
| User Identifier LDAP Attribute | Name of the LDAP attribute, which is used as a unique object identifier for objects in LDAP, objectSID for Active Directory or uidNumber of Open Ldap | uidNumber |
| Group Identifier LDAP Attribute | Name of the LDAP attribute, which is used as a unique object identifier for objects in LDAP, objectSID for Active Directory or gidNumber of Open Ldap | gidNumber |
| Modification Date Attribute | Name of the LDAP Attribute, which is used as the modification date for objects in LDAP              | modificationDate |
| Batch Size                  | Number of records                                                                                   | 1 |

:::warning

If the **UUID LDAP Attribute** does not exist, then the FULL DN is used as the unique identifier.

If the **Modification Date Attribute** does not exist, the extracted users cannot be versioned. Therefore, even if no modifications have been made to the users since the last extraction, all users will be extracted in the next execution.

:::

In the scope of this tutorial, the default values are correct and should not be updated.


The `Mapping Rules` tab contains the rules used by IdServer to map properties from OPENLDAP to user attributes / properties.

![Mapping rules](./images/ldap-4.png)

Before initiating the extraction, ensure that both the Identity Server and the Administration UI are running/launched.

Click on the `Histories` tab and then click on the `Launch` button.

Wait for a few seconds and then refresh the page. A new line should appear in the table indicating that 2 records have been extracted from LDAP.

![Exported users](./images/ldap-5.png)

## Import


Navigate to the `Identity Provisioning` screen and click on the `Import` button.

Wait for a few seconds and then refresh the page. A new line should appear in the table indicating that 2 records have been imported into the Identity Server.

Both users are visible on the Users screen.

![Users](./images/ldap-6.png)

## Authenticate


Browse the [Identity Server](https://localhost:5001/master) and click on the Authenticate button.

Authenticate using the following credentials. You are now authenticated with a user coming from OPENLDAP.

```
Login : firstUser
Password : password
```