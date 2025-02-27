# SCIM

The SCIM server acts as a central repository where different representations coexist, including Users or Groups.

It can be used by the [Identity Provider](../glossary) as a provisioning source. When a user is created or updated in SCIM, their local account in the [Identity Provider](../glossary) will be automatically created or updated.

Only version 2.0 of SCIM is supported.

## Storage

The following storage options are supported. Each NuGet package contains migration scripts to create or update the database structure.

<table>
    <thead>
        <tr><th>Name</th><th>Nuget package</th><th>JSON Path</th><th>Value</th></tr>
    </thead>
    <tbody>
        <tr>
            <td rowspan="2"><b>SQLServer</b></td>
            <td rowspan="2"><a href="https://www.nuget.org/packages/SimpleIdServer.Scim.SqlServerMigrations" target="_blank">SimpleIdServer.Scim.SqlServerMigrations</a></td>
            <td>$.StorageConfiguration.ConnectionString</td>
            <td></td>
        </tr>
        <tr>
            <td>$.StorageConfiguration.Type</td>
            <td>SQLSERVER</td>
        </tr>
        <tr>
            <td rowspan="2"><b>Postgresql</b></td>
            <td rowspan="2"><a href="https://www.nuget.org/packages/SimpleIdServer.Scim.PostgreMigrations" target="_blank">SimpleIdServer.Scim.PostgreMigrations</a></td>
            <td>$.StorageConfiguration.ConnectionString</td>
            <td></td>
        </tr>
        <tr>
            <td>$.StorageConfiguration.Type</td>
            <td>POSTGRE</td>
        </tr>
        <tr>
            <td rowspan="2"><b>Mongodb</b></td>
            <td rowspan="2"></td>
            <td>$.StorageConfiguration.ConnectionString</td>
            <td></td>
        </tr>
        <tr>
            <td>$.StorageConfiguration.Type</td>
            <td>MONGODB</td>
        </tr>
        <tr>
            <td rowspan="2"><b>MYSQL</b></td>
            <td rowspan="2"><a href="https://www.nuget.org/packages/SimpleIdServer.Scim.MySQLMigrations" target="_blank">SimpleIdServer.Scim.MySQLMigrations</a></td>
            <td>$.StorageConfiguration.ConnectionString</td>
            <td></td>
        </tr>
        <tr>
            <td>$.StorageConfiguration.Type</td>
            <td>MYSQL</td>
        </tr>
    </tbody>
</table>


## Azure LDAP Provisioning

To establish user provisioning from Azure Active Directory (AD) to the SCIM Endpoint, you can follow this tutorial [https://learn.microsoft.com/en-us/azure/active-directory/app-provisioning/use-scim-to-provision-users-and-groups#integrate-your-scim-endpoint-with-the-azure-ad-provisioning-service](https://learn.microsoft.com/en-us/azure/active-directory/app-provisioning/use-scim-to-provision-users-and-groups#integrate-your-scim-endpoint-with-the-azure-ad-provisioning-service).

When the provisioning mode is selected, the `Tenant URL` and `Secret Token` must be specified.

![Create Organization Unit](images/azuread-1.png)

Assign the value of the SCIM endpoint `http://localhost:5003` to the Tenant URL.
Set the Secret Token to one of the following values :

| Owner    | Value                                |
| -------- | ------------------------------------ |
| IdServer | ba521b3b-02f7-4a37-b03c-58f713bf88e7 |
| AzureAd  | 1595a72a-2804-495d-8a8a-2c861e7a736a |

:::warning

When the user is provisioned from Azure AD to the SCIM server, the following exception can occur because, according to the [RFC SCIM CORE SCHEMA](https://datatracker.ietf.org/doc/html/draft-ietf-scim-core-schema,), the schema for user representation does not include a `primary` property in the `addresses` section.

```
SimpleIdServer.Scim.Exceptions.SCIMSchemaViolatedException: attribute primary is not recognized by the SCIM schema
```

If this property is required, edit the `UserSchema.json` file and add the following property under the `addresses` section :

```
        {
          "name": "primary",
          "type": "boolean",
          "multiValued": false,
          "required": false,
          "mutability": "readWrite",
          "returned": "default",
          "uniqueness": "none",
          "description": "A Boolean value indicating the 'primary' or preferred attribute value for this attribute, e.g., the preferred mailing address or primary email address.  The primary attribute value 'true' MUST appear no more than once."
        }
```

:::

## Security

By default, the API is protected by an API Key. 
Any client wishing to execute an operation must include one of the following keys in the `Authorization` header.

| Owner    | Value                                |
| -------- | ------------------------------------ |
| IdServer | ba521b3b-02f7-4a37-b03c-58f713bf88e7 |
| AzureAd  | 1595a72a-2804-495d-8a8a-2c861e7a736a |

The keys have access to the following scopes

| Scope                | Description                       |
| -------------------- | --------------------------------- |
| query_scim_resource  | Retrieve resource attributes      |
| add_scim_resource    | Add resource                      |
| delete_scim_resource | Delete resource                   |
| update_scim_resource | Update resource                   |
| bulk_scim_resource   | Allows to execute bulk operations |

## SCIM - Example messages


### POST User

Minimal request for creating a user, username is the only mandatory field.

**Request**

```
HTTP POST: https://<domainUri>/Users

Authorization: Bearer ba521b3b-02f7-4a37-b03c-58f713bf88e7 

{
    "schemas": ["urn:ietf:params:scim:schemas:core:2.0:User"],
    "externalId": "external",
    "userName": "sid",
    "name": {
        "formatted": "formatted",
        "givenName": "givenName",
        "middleName": "middleName",
        "familyName": "familyName"
    }
}
```

**Response**

```
{
  "id": "6dc4ceb0-b376-4d4c-9ef0-e6c25e359754",
  "userName": "sid",
  "name": {
    "formatted": "formatted",
    "givenName": "givenName",
    "middleName": "middleName",
    "familyName": "familyName"
  },
  "groups": [],
  "photos": [],
  "roles": [],
  "emails": [],
  "x509Certificates": [],
  "ims": [],
  "addresses": [],
  "entitlements": [],
  "phoneNumbers": [],
  "meta": {
    "resourceType": "User",
    "created": "2023-06-12T11:12:45.0268402Z",
    "lastModified": "2023-06-12T11:12:45.0269172Z",
    "version": 0,
    "location": "https://localhost:5003/Users/6dc4ceb0-b376-4d4c-9ef0-e6c25e359754"
  },
  "externalId": "external",
  "schemas": [
    "urn:ietf:params:scim:schemas:core:2.0:User"
  ]
}
```

### GET User

#### Get one user

**Request**

```
HTTP GET: https://<domainUri>:5003/Users/<id>

Authorization: Bearer ba521b3b-02f7-4a37-b03c-58f713bf88e7
```

#### Search users

**Request**

```
HTTP GET: https://<domainUri>:5003/Users?filter=(emails[type eq "home"]) and (active eq false)

Authorization: Bearer ba521b3b-02f7-4a37-b03c-58f713bf88e7
``` 

The filter parameter must contain at least one valid expression. Each expression must contain an attribute name followed by an attribute operator and optional value.

The operators supported in the expression are listed below :

| Operator | Description              | Behavior                                                                                                                                                                             |
| -------- | ------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| eq       | equal                    | The attribute and operator values must be identical for a match.                                                                                                                     |
| ne       | not equal                | The attribute and operator values are not identical.                                                                                                                                 |
| co       | contains                 | The entire operator value must be a substring of the attribute value for a match.                                                                                                    |
| sw       | starts with              | The entire operator value must be a substring of the attribute value, starting at the beginning of the attribute value. The criterion is satisfied if the two strings are identical. |
| ew       | ends with                | The entire operator value must be a substring of the attribute value, matching at the end of the attribute value. The criterion is satisfied if the two strings are identical.       |
| pr       | present (has value )     | If the attribute has a non-empty or non-null value, or if it contains a non-empty node for complex attributes, there is a match.                                                     |
| gt       | greater than             | If the attribute value is greater than the operator value, there is a match.                                                                                                         |
| lt       | less than                | If the attribute value is less than the operator value, there is a match.                                                                                                            |
| le       | less than or equal to    | If the attribute value is less than or equal to the operator value, there is a match.                                                                                                |

### DELETE User

**Request**

```
HTTP DELETE: https://<domainUri>:5003/Users/<id>

Authorization: Bearer ba521b3b-02f7-4a37-b03c-58f713bf88e7
```

### PATCH user

#### Add one email address

**Request**

```
HTTP PATCH: https://<domainUri>:5003/Users/<id>

Authorization: Bearer ba521b3b-02f7-4a37-b03c-58f713bf88e7

{
    "schemas": [
        "urn:ietf:params:scim:api:messages:2.0:PatchOp"
    ],
    "Operations": [
        {
            "op": "add",
            "path": "emails",
            "value": [
                {
                    "primary": true,
                    "value": "sid@gmail.com",
                    "type": "home"
                }
            ]
        }
    ]
}
```


#### Remove one email address

**Request**

```
HTTP PATCH: https://<domainUri>:5003/Users/<id>

Authorization: Bearer ba521b3b-02f7-4a37-b03c-58f713bf88e7

{
    "schemas": [
        "urn:ietf:params:scim:api:messages:2.0:PatchOp"
    ],
    "Operations": [
        {
            "op": "remove",
            "path": "emails[type eq home]"
        }
    ]
}
```

#### Update Family name

**Request**

```
HTTP PATCH: https://<domainUri>:5003/Users/<id>

Authorization: Bearer ba521b3b-02f7-4a37-b03c-58f713bf88e7

{
    "schemas": [
        "urn:ietf:params:scim:api:messages:2.0:PatchOp"
    ],
    "Operations": [
        {
            "op": "replace",
            "path": "name.familyName",
            "value": "newfamilyname"
        }
    ]
}
```

### ADD Group

Request used for creating a group, displayName is the only mandatory field.

**Request**

```
HTTP PATCH: https://<domainUri>:5003/Groups

Authorization: Bearer ba521b3b-02f7-4a37-b03c-58f713bf88e7

{
    "schemas": ["urn:ietf:params:scim:schemas:core:2.0:Group"],
    "displayName": "administrator"
}
```

### PATCH Group

#### Assign one user to a group

**Request**

```
HTTP PATCH: https://<domainUri>:5003/Groups/<id>

Authorization: Bearer ba521b3b-02f7-4a37-b03c-58f713bf88e7

{   
    "schemas": [
        "urn:ietf:params:scim:api:messages:2.0:PatchOp"
    ],
    "Operations": [
        {
            "op": "add",
            "path": "members",
            "value": {
                "value": "{{userId}}"
            }
        }
    ]
}
```

The user representation will contain a reference to the group in its `groups` property.

#### Remove a user from a group

```
HTTP PATCH: https://<domainUri>:5003/Groups/<id>

Authorization: Bearer ba521b3b-02f7-4a37-b03c-58f713bf88e7

{   
    "schemas": [
        "urn:ietf:params:scim:api:messages:2.0:PatchOp"
    ],
    "Operations": [
        {
            "op": "remove",
            "path": "members[value eq {{userId}}]"
        }
    ]
}
```