Feature: Groups
	Check the /Groups endpoint

Scenario: Check users cannot be added twice to a group
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |
	And extract JSON from body
	And extract 'id' from JSON body into 'userid'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | Tour Guides                                       |
	| members     | [ { "value": "$userid$" } ]                       |
	And extract JSON from body
	And extract 'id' from JSON body	

	And execute HTTP PATCH JSON request 'http://localhost/Groups/$id$'
	| Key        | Value                                                                        |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                          |
	| Operations | [ { "op": "add", "path": "members", "value": [ { "value": "$userid$" } ] } ] |
	And extract JSON from body

	Then HTTP status code equals to '204'

Scenario: Check user can be added to a group	
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |

	And extract JSON from body
	And extract 'id' from JSON body
	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | Tour Guides                                       |
	| members     | [ { "value": "$id$" } ]                           |
	And execute HTTP GET request 'http://localhost/Users/$id$'	
	And extract JSON from body

	Then HTTP status code equals to '200'	
	Then HTTP HEADER contains 'Location'
	Then HTTP HEADER contains 'ETag'
	Then JSON exists 'id'
	Then JSON exists 'meta.created'
	Then JSON exists 'meta.lastModified'
	Then JSON exists 'meta.version'
	Then JSON exists 'meta.location'
	Then JSON exists 'groups[0].value'
	Then JSON 'groups[0].display'='Tour guides'

Scenario: Check user can be removed from a group (use filter)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |

	And extract JSON from body
	And extract 'id' from JSON body

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | Tour Guides                                       |
	| members     | [ { "value": "$id$" } ]                           |

	And extract JSON from body
	And extract 'id' from JSON body into 'groupId'
	
	And execute HTTP PATCH JSON request 'http://localhost/Groups/$groupId$'
	| Key        | Value                                                                        |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                          |
	| Operations | [ { "op": "remove", "path": "members" } ]								 	|

	And execute HTTP GET request 'http://localhost/Users/$id$'	
	And extract JSON from body

	Then HTTP status code equals to '200'	
	Then HTTP HEADER contains 'Location'
	Then HTTP HEADER contains 'ETag'
	Then JSON exists 'id'
	Then JSON exists 'meta.created'
	Then JSON exists 'meta.lastModified'
	Then JSON exists 'meta.version'
	Then JSON exists 'meta.location'
	Then 'groups' length is equals to '0'
	
Scenario: Check user can be removed from a group (use value)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |

	And extract JSON from body
	And extract 'id' from JSON body

	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen2                                                                                                          |
	| externalId     | externalid2                                                                                                    |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |

	And extract JSON from body
	And extract 'id' from JSON body into 'sid'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | Tour Guides                                       |
	| members     | [ { "value": "$id$" }, { "value": "$sid$" } ]     |

	And extract JSON from body
	And extract 'id' from JSON body into 'groupId'
	
	And execute HTTP PATCH JSON request 'http://localhost/Groups/$groupId$'
	| Key        | Value                                                                        |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                          |
	| Operations | [ { "op": "remove", "path": "members", "value" : [ { "value": "$id$" } ] } ]	|
	
	And execute HTTP GET request 'http://localhost/Groups/$groupId$'	
	And extract JSON from body

	Then HTTP status code equals to '200'	
	Then HTTP HEADER contains 'Location'
	Then HTTP HEADER contains 'ETag'
	Then 'members' length is equals to '1'


Scenario: Check group can be updated with multiple users
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |

	And extract JSON from body
	And extract 'id' from JSON body into 'firstuserid'
	And execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen2                                                                                                          |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |	
	And extract JSON from body
	And extract 'id' from JSON body into 'seconduserid'
	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | Tour Guides                                       |
	| members     | [ { "value": "$firstuserid$" } ]                  |
	And extract JSON from body
	And extract 'id' from JSON body into 'groupid'
	And execute HTTP PUT JSON request 'http://localhost/Groups/$groupid$'
	| Key     | Value                                                           |
	| schemas | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ]               |
	| members | [ { "value": "$firstuserid$" }, { "value": "$seconduserid$" } ] |
	And execute HTTP GET request 'http://localhost/Groups/$groupid$'	
	And extract JSON from body
	
	Then HTTP status code equals to '200'
	Then JSON exists 'members[0].value'
	Then JSON exists 'members[1].value'

Scenario: Update a group with two users (HTTP PATCH - replace)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen2                                                                                                          |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |

	And extract JSON from body
	And extract 'id' from JSON body into 'firstUser'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| members     | [ { "value" : "$firstUser$" } ]                   |

	And extract JSON from body
	And extract 'id' from JSON body into 'firstGroup'

	And execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen3                                                                                                          |
	| externalId     | externalid2                                                                                                    |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |

	And extract JSON from body
	And extract 'id' from JSON body into 'secondUser'
		
	And execute HTTP PATCH JSON request 'http://localhost/Groups/$firstGroup$'
	| Key        | Value                                                                                                            |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                                                              |
	| Operations | [ { "op": "replace", "path": "members", "value": [ { "value": "$firstUser$" }, { "value": "$secondUser$" } ] } ] |

	And execute HTTP GET request 'http://localhost/Groups/$firstGroup$'	
	And extract JSON from body
	
	Then HTTP status code equals to '200'
	Then 'members' length is equals to '2'

Scenario: Add a user to a group (HTTP PATCH)
	When execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | firstGroup                                        |

	And extract JSON from body
	And extract 'id' from JSON body into 'firstGroup'

	And execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen2                                                                                                          |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |

	And extract JSON from body
	And extract 'id' from JSON body into 'userId'
		
	And execute HTTP PATCH JSON request 'http://localhost/Groups/$firstGroup$'
	| Key        | Value                                                                        |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                          |
	| Operations | [ { "op": "add", "path": "members", "value": [ { "value": "$userId$" } ] } ] |

	And execute HTTP GET request 'http://localhost/Users/$userId$'	
	And extract JSON from body
	
	Then HTTP status code equals to '200'
	Then JSON 'groups[0].type'='direct'

Scenario: Remove a user from a group (HTTP PATCH)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen2                                                                                                          |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |

	And extract JSON from body
	And extract 'id' from JSON body into 'userId'

	When execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | firstGroup                                        |
	| members     | [ { "value": "$userId$" } ]                       |                                            

	And extract JSON from body
	And extract 'id' from JSON body into 'firstGroup'
		
	And execute HTTP PATCH JSON request 'http://localhost/Groups/$firstGroup$'
	| Key        | Value                                                    |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]      |
	| Operations | [ { "op": "remove", "path": "members" } ]				|

	And execute HTTP GET request 'http://localhost/Users/$userId$'	
	And extract JSON from body
	
	Then HTTP status code equals to '200'

Scenario: Add a user to a sub group and check user belongs to two groups (HTTP PATCH)
	When execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | firstGroup                                        |

	And extract JSON from body
	And extract 'id' from JSON body into 'firstGroup'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | secondGroup                                       |
	| members     | [ { "value": "$firstGroup$" } ]                   |

	And extract JSON from body
	And extract 'id' from JSON body into 'secondGroup'

	And execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen2                                                                                                          |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |

	And extract JSON from body
	And extract 'id' from JSON body into 'userId'
		
	And execute HTTP PATCH JSON request 'http://localhost/Groups/$firstGroup$'
	| Key        | Value                                                                        |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                          |
	| Operations | [ { "op": "add", "path": "members", "value": [ { "value": "$userId$" } ] } ] |

	And execute HTTP GET request 'http://localhost/Users/$userId$'	
	And extract JSON from body
	
	Then HTTP status code equals to '200'
	Then JSON 'groups[0].type'='direct'
	Then JSON 'groups[1].type'='indirect'

Scenario: Add a user to two sub groups and check the user belongs to three groups (HTTP PATCH)
	When execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | firstGroup                                        |

	And extract JSON from body
	And extract 'id' from JSON body into 'firstGroup'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | secondGroup                                       |
	| members     | [ { "value": "$firstGroup$" } ]                   |

	And extract JSON from body
	And extract 'id' from JSON body into 'secondGroup'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | thirdGroup                                        |
	| members     | [ { "value": "$firstGroup$" } ]                   |

	And extract JSON from body
	And extract 'id' from JSON body into 'thirdGroup'

	And execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen2                                                                                                          |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |

	And extract JSON from body
	And extract 'id' from JSON body into 'userId'
		
	And execute HTTP PATCH JSON request 'http://localhost/Groups/$firstGroup$'
	| Key        | Value                                                                        |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                          |
	| Operations | [ { "op": "add", "path": "members", "value": [ { "value": "$userId$" } ] } ] |

	And execute HTTP GET request 'http://localhost/Users/$userId$'	
	And extract JSON from body
	
	Then HTTP status code equals to '200'
	And 'groups' length is equals to '3'
	And JSON 'groups[0].type'='direct'
	And JSON 'groups[1].type'='indirect'
	And JSON 'groups[2].type'='indirect'

Scenario: Add one user into two groups
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen2                                                                                                          |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |

	And extract JSON from body
	And extract 'id' from JSON body into 'userId'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | firstGroup                                        |
	| members     | [ { "value": "$userId$" } ]                       |
	
	And extract JSON from body
	And extract 'id' from JSON body into 'firstGroup'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | secondGroup                                       |
	| members     | [ { "value": "$firstGroup$" } ]                   |

	And execute HTTP GET request 'http://localhost/Users/$userId$'	
	And extract JSON from body
	
	Then HTTP status code equals to '200'
	And 'groups' length is equals to '2'
	And JSON 'groups[0].type'='direct'
	And JSON 'groups[1].type'='indirect'
	
Scenario: Remove user from one group and check it has an indirect link to this group (HTTP PATCH)
	When execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | firstGroup                                        |

	And extract JSON from body
	And extract 'id' from JSON body into 'firstGroup'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | secondGroup                                       |
	| members     | [ { "value": "$firstGroup$" } ]                   |

	And extract JSON from body
	And extract 'id' from JSON body into 'secondGroup'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | thirdGroup                                        |
	| members     | [ { "value": "$firstGroup$" } ]                   |

	And extract JSON from body
	And extract 'id' from JSON body into 'thirdGroup'

	And execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen2                                                                                                          |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |

	And extract JSON from body
	And extract 'id' from JSON body into 'userId'
		
	And execute HTTP PATCH JSON request 'http://localhost/Groups/$firstGroup$'
	| Key        | Value                                                                        |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                          |
	| Operations | [ { "op": "add", "path": "members", "value": [ { "value": "$userId$" } ] } ] |
		
	And execute HTTP PATCH JSON request 'http://localhost/Groups/$secondGroup$'
	| Key        | Value                                                                        |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                          |
	| Operations | [ { "op": "add", "path": "members", "value": [ { "value": "$userId$" } ] } ] |
		
	And execute HTTP PATCH JSON request 'http://localhost/Groups/$thirdGroup$'
	| Key        | Value                                                                        |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                          |
	| Operations | [ { "op": "add", "path": "members", "value": [ { "value": "$userId$" } ] } ] |
		
	And execute HTTP PATCH JSON request 'http://localhost/Groups/$thirdGroup$'
	| Key        | Value                                                                        |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                          |
	| Operations | [ { "op": "remove", "path": "members[value eq $userId$]" } ]				    |

	And execute HTTP GET request 'http://localhost/Users/$userId$'	
	And extract JSON from body
	
	Then HTTP status code equals to '200'
	And 'groups' length is equals to '3'
	And JSON 'groups[0].type'='direct'
	And JSON 'groups[1].type'='direct'
	And JSON 'groups[2].type'='indirect'

Scenario: Remove all relationships and check user belongs to two groups (HTTP PATCH)
	When execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | firstGroup                                        |

	And extract JSON from body
	And extract 'id' from JSON body into 'firstGroup'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | secondGroup                                       |
	| members     | [ { "value": "$firstGroup$" } ]                   |

	And extract JSON from body
	And extract 'id' from JSON body into 'secondGroup'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | thirdGroup                                        |
	| members     | [ { "value": "$firstGroup$" } ]                   |

	And extract JSON from body
	And extract 'id' from JSON body into 'thirdGroup'

	And execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen2                                                                                                          |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |

	And extract JSON from body
	And extract 'id' from JSON body into 'userId'
		
	And execute HTTP PATCH JSON request 'http://localhost/Groups/$firstGroup$'
	| Key        | Value                                                                        |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                          |
	| Operations | [ { "op": "add", "path": "members", "value": [ { "value": "$userId$" } ] } ] |

	And execute HTTP PATCH JSON request 'http://localhost/Groups/$secondGroup$'
	| Key        | Value                                                                        |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                          |
	| Operations | [ { "op": "remove", "path": "members[value eq $firstGroup$]" } ]				|

	And execute HTTP PATCH JSON request 'http://localhost/Groups/$thirdGroup$'
	| Key        | Value                                                                        |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                          |
	| Operations | [ { "op": "remove", "path": "members[value eq $firstGroup$]" } ]				|

	And execute HTTP GET request 'http://localhost/Users/$userId$'	
	And extract JSON from body
	
	Then HTTP status code equals to '200'
	And 'groups' length is equals to '1'
	And JSON 'groups[0].type'='direct'

Scenario: Add a user to a sub group and check user belongs to two groups (HTTP PUT)
	When execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | firstGroup                                        |

	And extract JSON from body
	And extract 'id' from JSON body into 'firstGroup'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | secondGroup                                       |
	| members     | [ { "value": "$firstGroup$" } ]                   |

	And extract JSON from body
	And extract 'id' from JSON body into 'secondGroup'

	And execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen2                                                                                                          |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |

	And extract JSON from body
	And extract 'id' from JSON body into 'userId'
		
	And execute HTTP PUT JSON request 'http://localhost/Groups/$firstGroup$'
	| Key				| Value                                                                        |
	| schemas			| [ "urn:ietf:params:scim:schemas:core:2.0:Group" ]                            |
	| displayName		| firstGroup																   |
	| members			| [ { "value": "$userId$" } ]				                             	   |

	And execute HTTP GET request 'http://localhost/Users/$userId$'	
	And extract JSON from body
	
	Then HTTP status code equals to '200'
	Then JSON 'groups[0].type'='direct'
	Then JSON 'groups[1].type'='indirect'


Scenario: Add a user to two sub groups and check the user belongs to three groups (HTTP PUT)
	When execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | firstGroup                                        |

	And extract JSON from body
	And extract 'id' from JSON body into 'firstGroup'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | secondGroup                                       |
	| members     | [ { "value": "$firstGroup$" } ]                   |

	And extract JSON from body
	And extract 'id' from JSON body into 'secondGroup'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | thirdGroup                                        |
	| members     | [ { "value": "$firstGroup$" } ]                   |

	And extract JSON from body
	And extract 'id' from JSON body into 'thirdGroup'

	And execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen2                                                                                                          |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |

	And extract JSON from body
	And extract 'id' from JSON body into 'userId'
		
	And execute HTTP PUT JSON request 'http://localhost/Groups/$firstGroup$'
	| Key				| Value                                                                        |
	| schemas			| [ "urn:ietf:params:scim:schemas:core:2.0:Group" ]                            |
	| displayName		| firstGroup																   |
	| members			| [ { "value": "$userId$" } ]												   |

	And execute HTTP GET request 'http://localhost/Users/$userId$'	
	And extract JSON from body
	
	Then HTTP status code equals to '200'
	And 'groups' length is equals to '3'
	And JSON 'groups[0].type'='direct'
	And JSON 'groups[1].type'='indirect'
	And JSON 'groups[2].type'='indirect'

Scenario: Remove user from one group and check it has an indirect link to this group (HTTP PUT)
	When execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | firstGroup                                        |

	And extract JSON from body
	And extract 'id' from JSON body into 'firstGroup'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | secondGroup                                       |
	| members     | [ { "value": "$firstGroup$" } ]                   |

	And extract JSON from body
	And extract 'id' from JSON body into 'secondGroup'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | thirdGroup                                        |
	| members     | [ { "value": "$firstGroup$" } ]                   |

	And extract JSON from body
	And extract 'id' from JSON body into 'thirdGroup'

	And execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen2                                                                                                          |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |

	And extract JSON from body
	And extract 'id' from JSON body into 'userId'
		
	And execute HTTP PUT JSON request 'http://localhost/Groups/$firstGroup$'
	| Key				| Value                                                     |
	| schemas			| [ "urn:ietf:params:scim:schemas:core:2.0:Group" ]         |
	| displayName		| secondGroup												|
	| members			| [ { "value": "$userId$" } ]					            |

	And execute HTTP PUT JSON request 'http://localhost/Groups/$secondGroup$'
	| Key				| Value                                                     |
	| schemas			| [ "urn:ietf:params:scim:schemas:core:2.0:Group" ]         |
	| displayName		| secondGroup												|
	| members			| [ { "value": "$firstGroup$" }, { "value": "$userId$" } ]	|
		
	And execute HTTP PUT JSON request 'http://localhost/Groups/$thirdGroup$'
	| Key				| Value                                                     |
	| schemas			| [ "urn:ietf:params:scim:schemas:core:2.0:Group" ]         |
	| displayName		| thirdGroup												|
	| members			| [ { "value": "$firstGroup$" }, { "value": "$userId$" } ]	|
		
	And execute HTTP PUT JSON request 'http://localhost/Groups/$thirdGroup$'
	| Key				| Value                                                     |
	| schemas			| [ "urn:ietf:params:scim:schemas:core:2.0:Group" ]         |
	| displayName		| thirdGroup												|
	| members			| [ { "value": "$firstGroup$" } ]					        |

	And execute HTTP GET request 'http://localhost/Users/$userId$'	
	And extract JSON from body
	
	Then HTTP status code equals to '200'
	And 'groups' length is equals to '3'
	And JSON 'groups[0].type'='direct'
	And JSON 'groups[1].type'='direct'
	And JSON 'groups[2].type'='indirect'	

Scenario: Remove all relationships and check user belongs to one direct group (HTTP PUT)
	When execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | firstGroup                                        |

	And extract JSON from body
	And extract 'id' from JSON body into 'firstGroup'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | secondGroup                                       |
	| members     | [ { "value": "$firstGroup$" } ]                   |

	And extract JSON from body
	And extract 'id' from JSON body into 'secondGroup'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | thirdGroup                                        |
	| members     | [ { "value": "$firstGroup$" } ]                   |

	And extract JSON from body
	And extract 'id' from JSON body into 'thirdGroup'

	And execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen2                                                                                                          |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |

	And extract JSON from body
	And extract 'id' from JSON body into 'userId'
		
	And execute HTTP PUT JSON request 'http://localhost/Groups/$firstGroup$'
	| Key				| Value                                                                        |
	| schemas			| [ "urn:ietf:params:scim:schemas:core:2.0:Group" ]                            |
	| displayName		| firstGroup																   |
	| members			| [ { "value": "$userId$" } ]					                               |

	And execute HTTP PUT JSON request 'http://localhost/Groups/$secondGroup$'
	| Key				| Value                                                                        |
	| schemas			| [ "urn:ietf:params:scim:schemas:core:2.0:Group" ]                            |
	| displayName		| secondGroup																   |
	| members			| [ ]												                           |

	And execute HTTP PUT JSON request 'http://localhost/Groups/$thirdGroup$'
	| Key				| Value                                                                        |
	| schemas			| [ "urn:ietf:params:scim:schemas:core:2.0:Group" ]                            |
	| displayName		| thirdGroup																   |
	| members			| [ ]												                           |

	And execute HTTP GET request 'http://localhost/Users/$userId$'	
	And extract JSON from body
	
	Then HTTP status code equals to '200'
	And 'groups' length is equals to '1'
	And JSON 'groups[0].type'='direct'	

Scenario: Remove the nested group from the parent group and check user only has one group (HTTP PATCH)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen2                                                                                                          |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |

	And extract JSON from body
	And extract 'id' from JSON body into 'userId'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | firstGroup                                        |
	| members     | [ { "value": "$userId$" } ]                       |

	And extract JSON from body
	And extract 'id' from JSON body into 'firstGroup'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | secondGroup                                       |
	| members     | [ { "value": "$firstGroup$" } ]                   |

	And extract JSON from body
	And extract 'id' from JSON body into 'secondGroup'
	
	And execute HTTP PATCH JSON request 'http://localhost/Groups/$secondGroup$'
	| Key        | Value                                                   |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]     |
	| Operations | [ { "op": "remove", "path": "members" } ]               |

	And execute HTTP GET request 'http://localhost/Users/$userId$'	
	And extract JSON from body
	
	Then HTTP status code equals to '200'
	And 'groups' length is equals to '1'
	And JSON 'groups[0].type'='direct'	

Scenario: Remove the nested group from the parent group and check user only has one group (HTTP PUT)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen2                                                                                                          |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |

	And extract JSON from body
	And extract 'id' from JSON body into 'userId'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | firstGroup                                        |
	| members     | [ { "value": "$userId$" } ]                       |
	
	And extract JSON from body
	And extract 'id' from JSON body into 'firstGroup'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | secondGroup                                       |
	| members     | [ { "value": "$firstGroup$" } ]                   |

	And extract JSON from body
	And extract 'id' from JSON body into 'secondGroup'
	
	And execute HTTP PUT JSON request 'http://localhost/Groups/$secondGroup$'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | secondGroup                                       |
	| members     | [ ]                                               |

	And execute HTTP GET request 'http://localhost/Users/$userId$'	
	And extract JSON from body
	
	Then HTTP status code equals to '200'
	And 'groups' length is equals to '1'
	And JSON 'groups[0].type'='direct'	

Scenario: When parent's group 'displayName' is updated then user information must be updated (HTTP PATCH)
	When execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | firstGroup                                        |

	And extract JSON from body
	And extract 'id' from JSON body into 'firstGroup'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | secondGroup                                       |
	| members     | [ { "value": "$firstGroup$" } ]                   |

	And extract JSON from body
	And extract 'id' from JSON body into 'secondGroup'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | thirdGroup                                        |
	| members     | [ { "value": "$firstGroup$" } ]                   |

	And extract JSON from body
	And extract 'id' from JSON body into 'thirdGroup'

	And execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen2                                                                                                          |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |

	And extract JSON from body
	And extract 'id' from JSON body into 'userId'
		
	And execute HTTP PUT JSON request 'http://localhost/Groups/$firstGroup$'
	| Key				| Value                                                                        |
	| schemas			| [ "urn:ietf:params:scim:schemas:core:2.0:Group" ]                            |
	| displayName		| firstGroup																   |
	| members			| [ { "value": "$userId$" } ]												   |	
		
	And execute HTTP PATCH JSON request 'http://localhost/Groups/$secondGroup$'
	| Key        | Value                                                                     |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                       |
	| Operations | [ { "op": "replace", "path": "displayName", "value": "newDisplayName" } ] |

	And execute HTTP GET request 'http://localhost/Users/$userId$'	
	And extract JSON from body
	
	Then HTTP status code equals to '200'
	And 'groups' length is equals to '3'
	And JSON 'groups[0].type'='direct'
	And JSON 'groups[0].display'='firstGroup'
	And JSON 'groups[1].type'='indirect'
	And JSON 'groups[1].display'='newDisplayName'
	And JSON 'groups[2].type'='indirect'
	And JSON 'groups[2].display'='thirdGroup'

Scenario: When parent's group 'displayName' is updated then user information must be updated (HTTP PUT)
	When execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | firstGroup                                        |

	And extract JSON from body
	And extract 'id' from JSON body into 'firstGroup'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | secondGroup                                       |
	| members     | [ { "value": "$firstGroup$" } ]                   |

	And extract JSON from body
	And extract 'id' from JSON body into 'secondGroup'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | thirdGroup                                        |
	| members     | [ { "value": "$firstGroup$" } ]                   |

	And extract JSON from body
	And extract 'id' from JSON body into 'thirdGroup'

	And execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen2                                                                                                          |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |

	And extract JSON from body
	And extract 'id' from JSON body into 'userId'
		
	And execute HTTP PUT JSON request 'http://localhost/Groups/$firstGroup$'
	| Key				| Value                                                                        |
	| schemas			| [ "urn:ietf:params:scim:schemas:core:2.0:Group" ]                            |
	| displayName		| firstGroup																   |
	| members			| [ { "value": "$userId$" } ]												   |		
		
	And execute HTTP PUT JSON request 'http://localhost/Groups/$secondGroup$'
	| Key				| Value                                                |
	| schemas			| [ "urn:ietf:params:scim:schemas:core:2.0:Group" ]    |
	| members           | [ { "value": "$firstGroup$" } ]                      |
	| displayName       | newDisplayName                                       |

	And execute HTTP GET request 'http://localhost/Users/$userId$'	
	And extract JSON from body
	
	Then HTTP status code equals to '200'
	And 'groups' length is equals to '3'
	And JSON 'groups[0].type'='direct'
	And JSON 'groups[0].display'='firstGroup'
	And JSON 'groups[1].type'='indirect'
	And JSON 'groups[1].display'='newDisplayName'
	And JSON 'groups[2].type'='indirect'
	And JSON 'groups[2].display'='thirdGroup'

Scenario: When direct parent's group 'displayName' is updated then user information must be updated (HTTP PATCH)
	When execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | firstGroup                                        |

	And extract JSON from body
	And extract 'id' from JSON body into 'firstGroup'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | secondGroup                                       |
	| members     | [ { "value": "$firstGroup$" } ]                   |

	And extract JSON from body
	And extract 'id' from JSON body into 'secondGroup'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | thirdGroup                                        |
	| members     | [ { "value": "$firstGroup$" } ]                   |

	And extract JSON from body
	And extract 'id' from JSON body into 'thirdGroup'

	And execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen2                                                                                                          |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |

	And extract JSON from body
	And extract 'id' from JSON body into 'userId'
		
	And execute HTTP PUT JSON request 'http://localhost/Groups/$firstGroup$'
	| Key				| Value                                                                        |
	| schemas			| [ "urn:ietf:params:scim:schemas:core:2.0:Group" ]                            |
	| displayName		| firstGroup																   |
	| members			| [ { "value": "$userId$" } ]												   |	
		
	And execute HTTP PATCH JSON request 'http://localhost/Groups/$firstGroup$'
	| Key        | Value                                                                     |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                       |
	| Operations | [ { "op": "replace", "path": "displayName", "value": "newDisplayName" } ] |

	And execute HTTP GET request 'http://localhost/Users/$userId$'	
	And extract JSON from body
	
	Then HTTP status code equals to '200'
	And 'groups' length is equals to '3'
	And JSON 'groups[0].type'='direct'
	And JSON 'groups[0].display'='newDisplayName'
	And JSON 'groups[1].type'='indirect'
	And JSON 'groups[1].display'='secondGroup'
	And JSON 'groups[2].type'='indirect'
	And JSON 'groups[2].display'='thirdGroup'

Scenario: When direct parent's group 'displayName' is updated then user information must be updated (HTTP PUT)
	When execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | firstGroup                                        |

	And extract JSON from body
	And extract 'id' from JSON body into 'firstGroup'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | secondGroup                                       |
	| members     | [ { "value": "$firstGroup$" } ]                   |

	And extract JSON from body
	And extract 'id' from JSON body into 'secondGroup'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | thirdGroup                                        |
	| members     | [ { "value": "$firstGroup$" } ]                   |

	And extract JSON from body
	And extract 'id' from JSON body into 'thirdGroup'

	And execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen2                                                                                                          |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |

	And extract JSON from body
	And extract 'id' from JSON body into 'userId'
		
	And execute HTTP PUT JSON request 'http://localhost/Groups/$firstGroup$'
	| Key				| Value                                                                        |
	| schemas			| [ "urn:ietf:params:scim:schemas:core:2.0:Group" ]                            |
	| displayName		| firstGroup																   |
	| members			| [ { "value": "$userId$" } ]												   |
	
		
	And execute HTTP PUT JSON request 'http://localhost/Groups/$firstGroup$'
	| Key				| Value                                                |
	| schemas			| [ "urn:ietf:params:scim:schemas:core:2.0:Group" ]    |
	| members			| [ { "value": "$userId$" } ]					       |
	| displayName       | newDisplayName                                       |

	And execute HTTP GET request 'http://localhost/Users/$userId$'	
	And extract JSON from body
	
	Then HTTP status code equals to '200'
	And 'groups' length is equals to '3'
	And JSON 'groups[0].type'='direct'
	And JSON 'groups[0].display'='newDisplayName'
	And JSON 'groups[1].type'='indirect'
	And JSON 'groups[1].display'='secondGroup'
	And JSON 'groups[2].type'='indirect'
	And JSON 'groups[2].display'='thirdGroup'

Scenario: Add one sub group into a group and check user has an indirect link to the parent group (HTTP PATCH)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen2                                                                                                          |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |

	And extract JSON from body
	And extract 'id' from JSON body into 'userId'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | firstGroup                                        |
	| members     | [ { "value": "$userId$" } ]                       |

	And extract JSON from body
	And extract 'id' from JSON body into 'firstGroup'
	
	When execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | secondGroup                                       |

	And extract JSON from body
	And extract 'id' from JSON body into 'secondGroup'
		
	And execute HTTP PATCH JSON request 'http://localhost/Groups/$secondGroup$'
	| Key        | Value                                                                            |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                              |
	| Operations | [ { "op": "add", "path": "members", "value": [ { "value": "$firstGroup$" } ] } ] |

	And execute HTTP GET request 'http://localhost/Users/$userId$'	
	And extract JSON from body
	
	Then HTTP status code equals to '200'
	And 'groups' length is equals to '2'
	And JSON 'groups[0].type'='direct'
	And JSON 'groups[1].type'='indirect'

Scenario: Check member can be removed when full path is passed in the PATH remove operation
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |
	And extract JSON from body
	And extract 'id' from JSON body into 'userid'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | Tour Guides                                       |
	| members     | [ { "value": "$userid$" } ]                       |
	And extract JSON from body
	And extract 'id' from JSON body	

	And execute HTTP PATCH JSON request 'http://localhost/Groups/$id$'
	| Key        | Value                                                          |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]            |
	| Operations | [ { "op": "remove", "path": "members[value eq $userid$]" } ] |

	And execute HTTP GET request 'http://localhost/Groups/$id$'	
	And extract JSON from body
	
	Then HTTP status code equals to '200'
	And 'members' length is equals to '0'

Scenario: Update a group with an existing user and a new one
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | firstUser                                                                                                      |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |
	And extract JSON from body
	And extract 'id' from JSON body into 'firstUserId'
	
	And execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | secondUser                                                                                                     |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |
	And extract JSON from body
	And extract 'id' from JSON body into 'secondUserId'
	
	And execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | thirdUser                                                                                                      |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |
	And extract JSON from body
	And extract 'id' from JSON body into 'thirdUserId'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ]                                 |
	| displayName | Tour Guides														                  |
	| members     | [ { "value": "$firstUserId$" }, { "value" : "$secondUserId$", "type": "User" } ]  |
	And extract JSON from body
	And extract 'id' from JSON body into 'groupId'
	
	And execute HTTP PATCH JSON request 'http://localhost/Groups/$groupId$'
	| Key         | Value                                                                                                                 |
	| schemas     | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                                                                   |
	| Operations  | [ { "op": "replace", "path": "members", "value": [ { "value": "$secondUserId$" }, { "value" : "$thirdUserId$" } ] } ] |
	
	And execute HTTP GET request 'http://localhost/Groups/$groupId$'	
	And extract JSON from body

	Then '$.members' length is equals to '2'
	

Scenario: Assign a group to a hierarchy of groups and check the user belongs to the parent groups
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | firstUser                                                                                                      |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |
	And extract JSON from body
	And extract 'id' from JSON body into 'firstUserId'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                               |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ]   |
	| displayName | FirstGroup											|
	| members     | [ { "value": "$firstUserId$" } ]					|
	And extract JSON from body
	And extract 'id' from JSON body into 'firstGroupId'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                               |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ]   |
	| displayName | SecondGroup											|
	And extract JSON from body
	And extract 'id' from JSON body into 'secondGroupId'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                               |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ]   |
	| displayName | ThirdGroup											|
	| members     | [ { "value": "$secondGroupId$" } ]					|
	And extract JSON from body
	And extract 'id' from JSON body into 'thirdGroupId'

	And execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                               |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ]   |
	| displayName | ThirdGroup											|
	| members     | [ { "value": "$thirdGroupId$" } ]					|
	And extract JSON from body
	And extract 'id' from JSON body into 'fourthGroupId'
	
	And execute HTTP PATCH JSON request 'http://localhost/Groups/$secondGroupId$'
	| Key         | Value                                                                             |
	| schemas     | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                               |
	| Operations  | [ { "op": "add", "path": "members", "value": [ { "value": "$firstUserId$" } ] } ] |
	
	And execute HTTP GET request 'http://localhost/Users/$firstUserId$'	
	And extract JSON from body

	Then '$.groups' length is equals to '4'