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

	Then HTTP status code equals to '409'
	Then JSON 'response.status'='409'
	Then JSON 'response.scimType'='uniqueness'

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

Scenario: Check user can be removed from a group
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