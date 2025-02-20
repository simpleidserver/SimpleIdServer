Feature: UsersErrors
	Check the errors returned by the /Users endpoint

Scenario: Error is returned when filter is empty (HTTP GET)
	When execute HTTP GET request 'http://localhost/Users?startIndex=1&filter='
	
	And extract JSON from body
	Then HTTP status code equals to '400'
	Then JSON 'status'='400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'detail'='filter parameter must contains at least one valid expression'

Scenario: Error is returned when filter is empty (HTTP POST)
	When execute HTTP POST JSON request 'http://localhost/Users/.search'
	| Key              | Value	|
	| startIndex       | 1		|
	| filter           | |

	And extract JSON from body
	Then HTTP status code equals to '400'
	Then JSON 'status'='400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'detail'='filter parameter must contains at least one valid expression'
	
Scenario: Error is returned when startIndex <= 0
	When execute HTTP GET request 'http://localhost/Users?startIndex=0'
	
	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'status'='400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'detail'='startIndex must be >= 1'

Scenario: Error is returned when pass invalid JSON object (HTTP POST)
	When execute HTTP POST JSON request 'http://localhost/Users' with body '{ "schemas": [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ], "userName": "externalId": "externalId" }'
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='400'
	Then JSON 'scimType'='invalidSyntax'
	Then JSON 'detail'='HTTP POST request is not well formatted'

Scenario: Error is returned when value doesn't respect the type defined in the schema (HTTP PATCH)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key              | Value																												|
	| schemas          | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ]		|
	| userName         | bjen																												|
	| name             | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }								|
	| employeeNumber   | 100																												|

	And extract JSON from body
	And extract 'id' from JSON body	

	And execute HTTP PATCH JSON request 'http://localhost/Users/$id$'
	| Key              | Value                                                                                     |
	| schemas          | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                                       |
	| Operations	   | [ { "op": "replace", "path": "active", "value" : 1234 } ]								   |
	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='400'
	Then JSON 'scimType'='invalidValue'
	Then JSON 'detail'=''active' not valid boolean'

Scenario: Error is returned when trying to set null value to a required attribute (HTTP PATCH)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key              | Value																												|
	| schemas          | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ]		|
	| userName         | bjen																												|
	| name             | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }								|
	| employeeNumber   | 100																												|

	And extract JSON from body
	And extract 'id' from JSON body	

	And execute HTTP PATCH JSON request 'http://localhost/Users/$id$'
	| Key              | Value                                                                                     |
	| schemas          | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                                       |
	| Operations	   | [ { "op": "add", "path": "userName", "value" : "" } ]									   |
	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='400'
	Then JSON 'scimType'='invalidValue'
	Then JSON 'detail'='required attributes userName are missing'

Scenario: Error is returned when pass invalid JSON object (HTTP PATCH)
	When execute HTTP PATCH JSON request 'http://localhost/Users/id' with body '{ "schemas": [ "..." , "Operations": [ { "op": "replace", "path": "active", "value" : 234 } ]  }'
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='400'
	Then JSON 'scimType'='invalidSyntax'
	Then JSON 'detail'='HTTP PATCH request is not well formatted'
	
Scenario: Error is returned when pass invalid JSON object (HTTP PUT)
	When execute HTTP PUT JSON request 'http://localhost/Users/id' with body '{ "schemas": [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ], "userName": "externalId": "externalId" }'
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='400'
	Then JSON 'scimType'='invalidSyntax'
	Then JSON 'detail'='HTTP PUT request is not well formatted'

Scenario: Error is returned when pass invalid JSON object in sub attribute
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| employeeNumber | number                                                                                                         |
	| name           | {                                                                                                              |
	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='400'
	Then JSON 'scimType'='invalidValue'
	Then JSON 'detail'='{ is not a valid JSON'

Scenario: Error is returned when pass invalid boolean (HTTP POST)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| employeeNumber | number                                                                                                         |
	| active         | test                                                                                                           |
	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='400'
	Then JSON 'scimType'='invalidValue'
	Then JSON 'detail'=''test' not valid boolean'

Scenario: Error is returned when pass invalid decimal (HTTP POST)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| employeeNumber | number                                                                                                         |
	| age            | test                                                                                                           |
	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='400'
	Then JSON 'scimType'='invalidValue'
	Then JSON 'detail'=''test' not valid decimal'

Scenario: Error is returned when pass invalid DateTime (HTTP POST)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| employeeNumber | number                                                                                                         |
	| birthDate      | test                                                                                                           |
	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='400'
	Then JSON 'scimType'='invalidValue'
	Then JSON 'detail'=''test' not valid DateTime'

Scenario: Error is returned when pass invalid int (HTTP POST)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| employeeNumber | number                                                                                                         |
	| nbPoints       | test                                                                                                           |
	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='400'
	Then JSON 'scimType'='invalidValue'
	Then JSON 'detail'=''test' not valid integer'

Scenario: Error is returned when pass invalid Base64 (HTTP POST)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| employeeNumber | number                                                                                                         |
	| eidCertificate | %HELLO%                                                                                                        |
	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='400'
	Then JSON 'scimType'='invalidValue'
	Then JSON 'detail'=''%HELLO%' not valid Base64String'

Scenario: Error is returned when required attribute is missing (HTTP POST)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key     | Value                                                                                                          |
	| schemas | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='400'
	Then JSON 'scimType'='invalidValue'
	Then JSON 'detail'='required attributes urn:ietf:params:scim:schemas:core:2.0:User:userName are missing'

Scenario: Error is returned when required attribute is empty (HTTP POST)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       |                                                                                                                |
	| employeeNumber |                                                                                                                |
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='400'
	Then JSON 'scimType'='invalidValue'
	Then JSON 'detail'='required attributes urn:ietf:params:scim:schemas:core:2.0:User:userName are missing'

Scenario: Error is returned when schemas attribute is missing (HTTP POST)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key | Value |

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='400'
	Then JSON 'scimType'='invalidSyntax'
	Then JSON 'detail'='schemas attribute is missing'

Scenario: Error is returned when schema is not valid (HTTP POST)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key		| Value					|
	| schemas	| [ "invalidschema" ]	|

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='400'
	Then JSON 'scimType'='invalidSyntax'
	Then JSON 'detail'='the required schemas urn:ietf:params:scim:schemas:core:2.0:User,urn:ietf:params:scim:schemas:extension:enterprise:2.0:User are missing'

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
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='409'
	Then JSON 'scimType'='uniqueness'
	Then JSON 'detail'='attribute userName must be unique'

Scenario: Error is returned when the user doesn't exist (HTTP GET)
	When execute HTTP GET request 'http://localhost/Users/1'
	
	And extract JSON from body

	Then HTTP status code equals to '404'
	Then JSON 'status'='404'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'scimType'='unknown'
	Then JSON 'detail'='resource 1 not found'

Scenario: Error is returned when trying to remove an unknown user (HTTP DELETE)
	When execute HTTP DELETE request 'http://localhost/Users/1'
	
	And extract JSON from body

	Then HTTP status code equals to '404'
	Then JSON 'status'='404'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'detail'='resource 1 not found'

Scenario: Error is returned when schemas attribute is missing (HTTP PUT)
	When execute HTTP PUT JSON request 'http://localhost/Users/id'
	| Key | Value |

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='400'
	Then JSON 'scimType'='invalidSyntax'
	Then JSON 'detail'='schemas attribute is missing'

Scenario: Error is returned when schema is not valid (HTTP PUT)
	When execute HTTP PUT JSON request 'http://localhost/Users/id'
	| Key		| Value					|
	| schemas	| [ "invalidschema" ]	|

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='400'
	Then JSON 'scimType'='invalidSyntax'
	Then JSON 'detail'='the schemas invalidschema are unknown'

Scenario: Error is returned when trying to update an unknown resource (HTTP PUT)
	When execute HTTP PUT JSON request 'http://localhost/Users/id'
	| Key     | Value                                                                                                          |
	| schemas | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |

	And extract JSON from body

	Then HTTP status code equals to '404'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='404'
	Then JSON 'scimType'='unknown'
	Then JSON 'detail'='resource id not found'

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
	Then JSON 'scimType'='invalidValue'
	Then JSON 'detail'='required attributes urn:ietf:params:scim:schemas:core:2.0:User:userName are missing'
	

Scenario: Check immutable attribute cannot be updated with a different value
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |
	| immutable      | str																											  |
	
	And extract JSON from body
	And extract 'id' from JSON body	
	And execute HTTP PUT JSON request 'http://localhost/Users/$id$'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| employeeNumber | number                                                                                                         |
	| immutable      | str2                                                                                                           |
	
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='400'
	Then JSON 'scimType'='mutability'

Scenario: Error is returned when schemas attribute is missing (HTTP PATCH)
	When execute HTTP PATCH JSON request 'http://localhost/Users/id'
	| Key | Value |

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='400'
	Then JSON 'scimType'='invalidSyntax'
	Then JSON 'detail'='schemas attribute is missing'

	
Scenario: Error is returned when schema is not valid (HTTP PATCH)
	When execute HTTP PATCH JSON request 'http://localhost/Users/id'
	| Key		| Value					|
	| schemas	| [ "invalidschema" ]	|

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='400'
	Then JSON 'scimType'='invalidSyntax'
	Then JSON 'detail'='some schemas are not recognized by the endpoint'

Scenario: Error is returned when trying to patch an unknown resource (HTTP PATCH)
	When execute HTTP PATCH JSON request 'http://localhost/Users/id'
	| Key        | Value                                                            |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]              |
	| Operations	   | [ { "op": "replace", "path": "name", "value" : "" } ]	    |

	And extract JSON from body

	Then HTTP status code equals to '404'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='404'
	Then JSON 'scimType'='unknown'
	Then JSON 'detail'='resource id not found'

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
	Then JSON 'scimType'='noTarget'
	Then JSON 'detail'='path  is not valid'

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
	Then JSON 'scimType'='noTarget'
	Then JSON 'detail'='attribute fakepath is not recognized by the SCIM schema'


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
	| Key        | Value                                                                                                            |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                                                              |
	| Operations | [ { "op" : "replace", "path": "phones[phoneNumber eq 03]", "value" : { "phoneNumber": "03", "type": "test" } } ] |

	And extract JSON from body
	Then HTTP status code equals to '400'
	Then JSON 'status'='400'
	Then JSON 'scimType'='noTarget'
	Then JSON 'detail'='PATCH can be applied only on existing attributes'

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
	Then JSON 'status'='400'
	Then JSON 'scimType'='invalidSyntax'
	Then JSON 'detail'=''PATCH' request is not well-formatted'

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
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='400'
	Then JSON 'scimType'='invalidValue'
	Then JSON 'detail'='property type is not a valid canonical value'

Scenario: Error is returned when entitlment is added twice
	When execute HTTP POST JSON request 'http://localhost/CustomUsers'
	| Key      | Value                     |
	| schemas  | [ "urn:customuser" ]      |
	| userName | userName                  |
	
	And extract JSON from body
	And extract 'id' from JSON body into 'userId'

	And execute HTTP POST JSON request 'http://localhost/Entitlements'
	| Key         | Value                 |
	| schemas     | [ "urn:entitlement" ] |
	| displayName | firstEntitlement      |

	And extract JSON from body
	And extract 'id' from JSON body into 'firstEntitlement'

	And execute HTTP POST JSON request 'http://localhost/Entitlements'
	| Key         | Value                 |
	| schemas     | [ "urn:entitlement" ] |
	| displayName | secondEntitlement     |

	And extract JSON from body
	And extract 'id' from JSON body into 'secondEntitlement'

	And execute HTTP PATCH JSON request 'http://localhost/CustomUsers/$userId$'
	| Key        | Value                                                                                                                            |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                                                                              |
	| Operations | [ { "op": "add", "path": "entitlements", "value" : [ { "value": "$firstEntitlement$" }, { "value": "$secondEntitlement$" } ] } ] |
	
	And execute HTTP PATCH JSON request 'http://localhost/CustomUsers/$userId$'
	| Key        | Value                                                                                                                            |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                                                                              |
	| Operations | [ { "op": "add", "path": "entitlements", "value" : [ { "value": "$firstEntitlement$" }, { "value": "$secondEntitlement$" } ] } ] |

	And extract JSON from body

	Then HTTP status code equals to '204'

Scenario: Error is returned when emails.value is not passed
	When execute HTTP POST JSON request 'http://localhost/CustomUsers'
	| Key      | Value                      |
	| schemas  | [ "urn:customuser" ]       |
	| userName | userName                   |
	| emails   | [ { "primary" : "true" } ] |	
	
	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='400'
	Then JSON 'scimType'='invalidValue'
	Then JSON 'detail'='required attributes urn:customuser:emails.value are missing'

Scenario: Error is returned when endpoint is unknown
	When execute HTTP GET request 'http://localhost/unknown'
	
	And extract JSON from body

	Then HTTP status code equals to '404'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'detail'='Endpoint not found'
	Then JSON 'scimType'='unknown'

Scenario: Error is returned when name is empty (HTTP PATCH)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key              | Value																												|
	| schemas          | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ]		|
	| userName         | bjen																												|
	| name             | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }								|
	| employeeNumber   | 100																												|

	And extract JSON from body
	And extract 'id' from JSON body	

	And execute HTTP PATCH JSON request 'http://localhost/Users/$id$'
	| Key              | Value                                                      |
	| schemas          | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]        |
	| Operations	   | [ { "op": "replace", "path": "name", "value" : "" } ]	    |
	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='400'
	Then JSON 'scimType'='invalidValue'
	Then JSON 'detail'='name is not a valid JSON'

Scenario: Error is returned when Operations is empty
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key              | Value																											 |
	| schemas          | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ]	 |
	| userName         | bjen																											 |
	| name             | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }							 |
	| employeeNumber   | 100																											 |

	And extract JSON from body
	And extract 'id' from JSON body	

	And execute HTTP PATCH JSON request 'http://localhost/Users/$id$'
	| Key              | Value                                                      |
	| schemas          | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]        |
	| Operations	   | [ ]	                                                    |
	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='400'
	Then JSON 'scimType'='invalidSyntax'
	Then JSON 'detail'='At least one operation must be passed'

Scenario: Error is returned when required attribute is not passed (HTTP PATCH)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key              | Value																											 |
	| schemas          | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ]	 |
	| userName         | bjen																											 |
	| name             | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }							 |
	| employeeNumber   | 100																											 |

	And extract JSON from body
	And extract 'id' from JSON body

	And execute HTTP PATCH JSON request 'http://localhost/Users/$id$'
	| Key              | Value                                                 |
	| schemas          | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]   |
	| Operations	   | [ { "op": "replace", "path" : "name.givenName" } ]    |

	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='400'
	Then JSON 'scimType'='invalidvalue'
	Then JSON 'detail'='name.givenName is not a valid string'

Scenario: Error is returned when trying to remove a required attribute
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key              | Value																												|
	| schemas          | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ]		|
	| userName         | bjen																												|
	| name             | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }								|
	| employeeNumber   | 100																												|

	And extract JSON from body
	And extract 'id' from JSON body	

	And execute HTTP PATCH JSON request 'http://localhost/Users/$id$'
	| Key              | Value                                                  |
	| schemas          | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]    |
	| Operations	   | [ { "op": "remove", "path": "employeeNumber" } ]		|
	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='400'
	Then JSON 'scimType'='mutability'
	Then JSON 'detail'='Required Attributes employeeNumber cannot be removed'

Scenario: Error is returned when trying to remove a READONLY attribute
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
	And extract 'id' from JSON body into 'groupId'
	
	And execute HTTP PATCH JSON request 'http://localhost/Users/$userid$'
	| Key        | Value                                                                    |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                      |
	| Operations | [ { "op": "remove", "path": "groups[value eq \"$groupId$\"].type" } ]	|

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='400'
	Then JSON 'scimType'='mutability'
	Then JSON 'detail'='Readonly Attributes groups.type cannot be removed'