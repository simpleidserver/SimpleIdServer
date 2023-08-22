# SCIM - Example messages

## POST User

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

## GET User

### Get one user

**Request**

```
HTTP GET: https://<domainUri>:5003/Users/<id>

Authorization: Bearer ba521b3b-02f7-4a37-b03c-58f713bf88e7
```

### Search users

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

## DELETE User

**Request**

```
HTTP DELETE: https://<domainUri>:5003/Users/<id>

Authorization: Bearer ba521b3b-02f7-4a37-b03c-58f713bf88e7
```

## PATCH user

### Add one email address

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


### Remove one email address

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

### Update Family name

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

## ADD Group

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

## PATCH Group

### Assign one user to a group

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

### Remove a user from a group

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