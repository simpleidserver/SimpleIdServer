Feature: Groups
	Check the /Groups endpoint

Scenario: Check Group can be created
	When execute HTTP POST JSON request 'http://localhost/Groups'
	| Key         | Value                                                                                                                                                           |
	| schemas     | [ "urn:ietf:params:scim:schemas:core:2.0:Group" ]                                                                                                               |
	| displayName | Tour Guides                                                                                                                                                     |
	| members     | [ { "value": "2819c223-7f76-453a-919d-413861904646", "$ref": "https://example.com/v2/Users/2819c223-7f76-453a-919d-413861904646", "display": "Babs Jensen" }  ] |
	And extract JSON from body
	And extract 'id' from JSON body	
	And execute HTTP PATCH JSON request 'http://localhost/Groups/$id$'
	| Key        | Value                                                                                                                                                                                                                                                                                                          |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                                                                                                                                                                                                                                                            |
	| Operations | [ { "op" : "remove", "path": "members[display eq \"Babs Jensen\" and value co \"2819\"]" }, { "op": "add", "path": "members", "value": [ { "value": "902c246b-6245-4190-8e05-00816be7344a", "$ref": "https://example.com/v2/Users/902c246b-6245-4190-8e05-00816be7344a", "display": "Mandy Pepperidge" } ] } ] |
	And execute HTTP GET request 'http://localhost/Groups/$id$'	
	And extract JSON from body

	Then HTTP status code equals to '200'	
	Then HTTP HEADER contains 'Location'
	Then HTTP HEADER contains 'ETag'
	Then JSON exists 'id'
	Then JSON exists 'meta.created'
	Then JSON exists 'meta.lastModified'
	Then JSON exists 'meta.version'
	Then JSON exists 'meta.location'
	Then JSON 'displayName'='Tour Guides'
	Then JSON 'members[0].display'='Mandy Pepperidge'
	Then JSON 'members[0].$ref'='https://example.com/v2/Users/902c246b-6245-4190-8e05-00816be7344a'
	Then JSON 'members[0].value'='902c246b-6245-4190-8e05-00816be7344a'