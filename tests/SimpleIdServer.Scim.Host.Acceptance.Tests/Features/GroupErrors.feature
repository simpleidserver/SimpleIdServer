Feature: GroupErrors
	Check the errors returned by the /Groups endpoint

Scenario: Check group cannot have a self reference
	When execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                             |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ] |
	| displayName | Tour Guides                                       |
	And extract JSON from body
	And extract 'id' from JSON body	

	And execute HTTP PATCH JSON request 'http://localhost/Groups/$id$'
	| Key        | Value                                                                    |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                      |
	| Operations | [ { "op": "add", "path": "members", "value": [ { "value": "$id$" } ] } ] |
	And extract JSON from body

	Then HTTP status code equals to '500'
	Then JSON 'detail'='Representation cannot have a self reference'

Scenario: When group doesn't exist and the parameter exclusedAttributes is passed into the request then an error 404 is returned
	When execute HTTP GET request 'http://localhost/Groups/invalid?excludedAttributes=members'
	
	And extract JSON from body
	
	Then HTTP status code equals to '404'
	Then JSON 'status'='404'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'scimType'='unknown'
	Then JSON 'detail'='resource invalid not found'