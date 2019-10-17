Feature: Schemas
	Check the /Bulk endpoint
		
Scenario: Check all schemas are returned
	When execute HTTP GET request 'http://localhost/Schemas'
	
	And extract JSON from body

	Then HTTP status code equals to '200'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:ListResponse'
	Then JSON 'Resources[0].id'='urn:ietf:params:scim:schemas:core:2.0:User'
	Then JSON 'Resources[1].id'='urn:ietf:params:scim:schemas:core:2.0:Group'