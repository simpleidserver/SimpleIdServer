Feature: Schemas
	Check the /Bulk endpoint
		
Scenario: Check all schemas are returned
	When execute HTTP GET request 'http://localhost/Schemas'
	
	And extract JSON from body

	Then HTTP status code equals to '200'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:ListResponse'
	Then JSON 'Resources[0].id'='urn:ietf:params:scim:schemas:core:2.0:User'
	Then JSON 'Resources[0].name'='User'
	Then JSON 'Resources[0].description'='User Account'
	Then JSON 'Resources[0].attributes[0].mutability'='readWrite'
	Then JSON 'Resources[1].id'='urn:ietf:params:scim:schemas:extension:enterprise:2.0:User'
	Then JSON 'Resources[1].name'='EnterpriseUser'
	Then JSON 'Resources[2].id'='urn:ietf:params:scim:schemas:core:2.0:Group'
	Then JSON 'Resources[2].name'='Group'
	Then JSON 'Resources[2].description'='Group'