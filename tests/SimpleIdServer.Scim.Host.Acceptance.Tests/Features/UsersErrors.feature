Feature: UsersErrors
	Check the errors returned by the /Users endpoint

Scenario: Error is returned when pass invalid JSON object (HTTP POST)
	When execute HTTP POST JSON request 'http://localhost/Users' with body '{ "schemas": [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ], "userName": "externalId": "externalId" }'
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'response.schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'response.status'='400'
	Then JSON 'response.scimType'='invalidSyntax'
	Then JSON 'response.detail'='HTTP POST request is not well formatted'
	
Scenario: Error is returned when pass invalid JSON object (HTTP PUT)
	When execute HTTP PUT JSON request 'http://localhost/Users/id' with body '{ "schemas": [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ], "userName": "externalId": "externalId" }'
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'response.schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'response.status'='400'
	Then JSON 'response.scimType'='invalidSyntax'
	Then JSON 'response.detail'='HTTP PUT request is not well formatted'

Scenario: Error is returned when pass invalid JSON object in sub attribute
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| employeeNumber | number                                                                                                         |
	| name           | {                                                                                                              |
	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'response.schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'response.status'='400'
	Then JSON 'response.scimType'='schemaViolated'
	Then JSON 'response.detail'='{ is not a valid JSON'

Scenario: Error is returned when pass invalid boolean (HTTP POST)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| employeeNumber | number                                                                                                         |
	| active         | test                                                                                                           |
	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'response.schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'response.status'='400'
	Then JSON 'response.scimType'='schemaViolated'
	Then JSON 'response.detail'=''test' are not valid boolean'

Scenario: Error is returned when pass invalid decimal (HTTP POST)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| employeeNumber | number                                                                                                         |
	| age            | test                                                                                                           |
	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'response.schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'response.status'='400'
	Then JSON 'response.scimType'='schemaViolated'
	Then JSON 'response.detail'=''test' are not valid decimal'

Scenario: Error is returned when pass invalid DateTime (HTTP POST)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| employeeNumber | number                                                                                                         |
	| birthDate      | test                                                                                                           |
	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'response.schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'response.status'='400'
	Then JSON 'response.scimType'='schemaViolated'
	Then JSON 'response.detail'=''test' are not valid DateTime'

Scenario: Error is returned when pass invalid int (HTTP POST)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| employeeNumber | number                                                                                                         |
	| nbPoints       | test                                                                                                           |
	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'response.schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'response.status'='400'
	Then JSON 'response.scimType'='schemaViolated'
	Then JSON 'response.detail'=''test' are not valid integer'

Scenario: Error is returned when pass invalid Base64 (HTTP POST)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| employeeNumber | number                                                                                                         |
	| eidCertificate | %HELLO%                                                                                                        |
	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'response.schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'response.status'='400'
	Then JSON 'response.scimType'='schemaViolated'
	Then JSON 'response.detail'=''%HELLO%' are not valid Base64String'

Scenario: Error is returned when required attribute is missing (HTTP POST)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key     | Value                                                                                                          |
	| schemas | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'response.schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'response.status'='400'
	Then JSON 'response.scimType'='schemaViolated'
	Then JSON 'response.detail'='required attributes urn:ietf:params:scim:schemas:core:2.0:User:userName are missing'

Scenario: Error is returned when required attribute is empty (HTTP POST)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       |                                                                                                                |
	| employeeNumber |                                                                                                                |
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'response.schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'response.status'='400'
	Then JSON 'response.scimType'='schemaViolated'
	Then JSON 'response.detail'='required attributes urn:ietf:params:scim:schemas:core:2.0:User:userName are missing'

Scenario: Error is returned when schemas attribute is missing (HTTP POST)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key | Value |

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'status'='400'
	Then JSON 'response.schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'response.status'='400'
	Then JSON 'response.scimType'='invalidSyntax'
	Then JSON 'response.detail'='schemas attribute is missing'

Scenario: Error is returned when schema is not valid (HTTP POST)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key		| Value					|
	| schemas	| [ "invalidschema" ]	|

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'status'='400'
	Then JSON 'response.schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'response.status'='400'
	Then JSON 'response.scimType'='invalidSyntax'
	Then JSON 'response.detail'='the required schemas urn:ietf:params:scim:schemas:core:2.0:User,urn:ietf:params:scim:schemas:extension:enterprise:2.0:User are missing'

Scenario: Error is returned when trying to add two resources with the same unique attribute
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |

	
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |

	And extract JSON from body

	Then HTTP status code equals to '409'
	Then JSON 'status'='409'
	Then JSON 'response.schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'response.status'='409'
	Then JSON 'response.scimType'='uniqueness'
	Then JSON 'response.detail'='attribute userName must be unique'

Scenario: Error is returned when the user doesn't exist (HTTP GET)
	When execute HTTP GET request 'http://localhost/Users/1'
	
	And extract JSON from body

	Then HTTP status code equals to '404'
	Then JSON 'status'='404'
	Then JSON 'response.schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'response.detail'='resource 1 not found'

Scenario: Error is returned when trying to remove an unknown user (HTTP DELETE)
	When execute HTTP DELETE request 'http://localhost/Users/1'
	
	And extract JSON from body

	Then HTTP status code equals to '404'
	Then JSON 'status'='404'
	Then JSON 'response.schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'response.detail'='resource 1 not found'

Scenario: Error is returned when schemas attribute is missing (HTTP PUT)
	When execute HTTP PUT JSON request 'http://localhost/Users/id'
	| Key | Value |

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'status'='400'
	Then JSON 'response.schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'response.status'='400'
	Then JSON 'response.scimType'='invalidSyntax'
	Then JSON 'response.detail'='schemas attribute is missing'

Scenario: Error is returned when schema is not valid (HTTP PUT)
	When execute HTTP PUT JSON request 'http://localhost/Users/id'
	| Key		| Value					|
	| schemas	| [ "invalidschema" ]	|

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'status'='400'
	Then JSON 'response.schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'response.status'='400'
	Then JSON 'response.scimType'='invalidSyntax'
	Then JSON 'response.detail'='the schemas invalidschema are unknown'

Scenario: Error is returned when trying to update an unknown resource (HTTP PUT)
	When execute HTTP PUT JSON request 'http://localhost/Users/id'
	| Key     | Value                                                                                                          |
	| schemas | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |

	And extract JSON from body

	Then HTTP status code equals to '404'
	Then JSON 'status'='404'
	Then JSON 'response.schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'response.status'='404'
	Then JSON 'response.scimType'='unknown'
	Then JSON 'response.detail'='resource id not found'

Scenario: Error is returned when update and required attribute is missing (HTTP PUT)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |
	
	And extract JSON from body
	And extract 'id' from JSON body	
	And execute HTTP PUT JSON request 'http://localhost/Users/$id$'
	| Key            | Value                                            |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User" ] |
	| employeeNumber | 01                                               |

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'status'='400'
	Then JSON 'response.status'='400'
	Then JSON 'response.scimType'='schemaViolated'
	Then JSON 'response.detail'='required attributes urn:ietf:params:scim:schemas:core:2.0:User:userName are missing'
	

Scenario: Error is returned when update an immutable attribute (HTTP PUT)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |
	
	And extract JSON from body
	And extract 'id' from JSON body	
	And execute HTTP PUT JSON request 'http://localhost/Users/$id$'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| employeeNumber | number                                                                                                         |
	| immutable      | str                                                                                                            |
	
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'status'='400'
	Then JSON 'response.schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'response.status'='400'
	Then JSON 'response.scimType'='mutability'

Scenario: Error is returned when schemas attribute is missing (HTTP PATCH)
	When execute HTTP PATCH JSON request 'http://localhost/Users/id'
	| Key | Value |

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'status'='400'
	Then JSON 'response.schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'response.status'='400'
	Then JSON 'response.scimType'='invalidSyntax'
	Then JSON 'response.detail'='schemas attribute is missing'

	
Scenario: Error is returned when schema is not valid (HTTP PATCH)
	When execute HTTP PATCH JSON request 'http://localhost/Users/id'
	| Key		| Value					|
	| schemas	| [ "invalidschema" ]	|

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'status'='400'
	Then JSON 'response.schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'response.status'='400'
	Then JSON 'response.scimType'='invalidSyntax'
	Then JSON 'response.detail'='some schemas are not recognized by the endpoint'

Scenario: Error is returned when trying to patch an unknown resource (HTTP PATCH)
	When execute HTTP PATCH JSON request 'http://localhost/Users/id'
	| Key        | Value                                               |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ] |
	| Operations | [ ]                                                 |

	And extract JSON from body

	Then HTTP status code equals to '404'
	Then JSON 'status'='404'
	Then JSON 'response.schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'response.status'='404'
	Then JSON 'response.scimType'='unknown'
	Then JSON 'response.detail'='resource id not found'

Scenario: Error is returned when trying to remove attribute and path is not specified (HTTP PATCH)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| phones         | [ { "phoneNumber": "01", "type": "mobile" }, { "phoneNumber": "02", "type": "home" } ]                         |
	| employeeNumber | number                                                                                                         |
	| scores         | { "math" : [ { "score" : "10" } ] }                                                                            |
	| roles          | [ "role1", "role2" ]                                                                                           |
	
	And extract JSON from body
	And extract 'id' from JSON body	
	And execute HTTP PATCH JSON request 'http://localhost/Users/$id$'
	| Key        | Value                                               |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ] |
	| Operations | [ { "op" : "remove" } ]                             |

	And extract JSON from body
	Then HTTP status code equals to '400'
	Then JSON 'status'='400'
	Then JSON 'response.status'='400'
	Then JSON 'response.scimType'='noTarget'
	Then JSON 'response.detail'='path  is not valid'

Scenario: Error is returned when trying to add attribute and path is not valid (HTTP PATCH)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| phones         | [ { "phoneNumber": "01", "type": "mobile" }, { "phoneNumber": "02", "type": "home" } ]                         |
	| employeeNumber | number                                                                                                         |
	| scores         | { "math" : [ { "score" : "10" } ] }                                                                            |
	| roles          | [ "role1", "role2" ]                                                                                           |

	And extract JSON from body
	And extract 'id' from JSON body	
	And execute HTTP PATCH JSON request 'http://localhost/Users/$id$'
	| Key        | Value                                               |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ] |
	| Operations | [ { "op" : "add", "path": "fakepath" } ]            |
	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'status'='400'
	Then JSON 'response.status'='400'
	Then JSON 'response.scimType'='noTarget'
	Then JSON 'response.detail'='attribute fakepath is not recognized by the SCIM schema'


Scenario: Error is returned when trying to PATCH and there is no match (HTTP PATCH)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| phones         | [ { "phoneNumber": "01", "type": "mobile" }, { "phoneNumber": "02", "type": "home" } ]                         |
	| employeeNumber | number                                                                                                         |
	| scores         | { "math" : [ { "score" : "10" } ] }                                                                            |
	| roles          | [ "role1", "role2" ]                                                                                           |
	
	And extract JSON from body
	And extract 'id' from JSON body	
	And execute HTTP PATCH JSON request 'http://localhost/Users/$id$'
	| Key        | Value                                                         |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]           |
	| Operations | [ { "op" : "replace", "path": "phones[phoneNumber eq 03]" } ] |

	And extract JSON from body
	Then HTTP status code equals to '400'
	Then JSON 'status'='400'
	Then JSON 'response.status'='400'
	Then JSON 'response.scimType'='noTarget'
	Then JSON 'response.detail'='PATCH can be applied only on existing attributes'

Scenario: Error is returned when "op" value is invalid (HTTP PATCH)
		When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| phones         | [ { "phoneNumber": "01", "type": "mobile" }, { "phoneNumber": "02", "type": "home" } ]                         |
	| employeeNumber | number                                                                                                         |
	| scores         | { "math" : [ { "score" : "10" } ] }                                                                            |
	| roles          | [ "role1", "role2" ]                                                                                           |
	 
	And extract JSON from body
	And extract 'id' from JSON body	
	And execute HTTP PATCH JSON request 'http://localhost/Users/$id$'
	| Key        | Value                                                            |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]              |
	| Operations | [ { "op" : "invalid-op", "path": "phones[phoneNumber eq 03]" } ] |

	And extract JSON from body
	Then HTTP status code equals to '400'
	Then JSON 'response.status'='400'
	Then JSON 'response.scimType'='invalidSyntax'
	Then JSON 'response.detail'=''PATCH' request is not well-formatted'

Scenario: Error is returned when trying to add a none canonical value (HTTP POST)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| externalId     | externalid                                                                                                     |
	| type           | unsupported                                                                                                    |
	| employeeNumber | number                                                                                                         |
	
	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'response.schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'response.status'='400'
	Then JSON 'response.scimType'='schemaViolated'
	Then JSON 'response.detail'='property type is not a valid canonical value'
