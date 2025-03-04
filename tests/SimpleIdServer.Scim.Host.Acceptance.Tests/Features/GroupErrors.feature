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