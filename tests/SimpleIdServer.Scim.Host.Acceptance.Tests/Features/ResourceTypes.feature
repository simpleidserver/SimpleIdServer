Feature: ResourceTypes
	Check the /ResourceTypes endpoint
		
Scenario: Check informations returned by ResourceTypes endpoint
	When execute HTTP GET request 'http://localhost/ResourceTypes'

	And extract JSON from body
	
	Then HTTP status code equals to '200'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:ListResponse'
	Then JSON 'startIndex'='1'
	Then JSON 'Resources[0].id'='User'
	Then JSON 'Resources[0].name'='User'
	Then JSON 'Resources[0].description'='User Account'
	Then JSON 'Resources[0].endpoint'='/Users'
	Then JSON 'Resources[0].schema'='urn:ietf:params:scim:schemas:core:2.0:User'
	Then JSON 'Resources[0].schemaExtensions[0].required'='true'
	Then JSON 'Resources[0].schemaExtensions[0].schema'='urn:ietf:params:scim:schemas:extension:enterprise:2.0:User'
	Then JSON 'Resources[0].meta.resourceType'='ResourceType'
	Then JSON 'Resources[1].id'='Group'
	Then JSON 'Resources[1].name'='Group'
	Then JSON 'Resources[1].description'='Group'
	Then JSON 'Resources[1].endpoint'='/Groups'
	Then JSON 'Resources[1].schema'='urn:ietf:params:scim:schemas:core:2.0:Group'
	Then JSON 'Resources[1].meta.resourceType'='ResourceType'

Scenario: Get ResourceType and check their informations
	When execute HTTP GET request 'http://localhost/ResourceTypes/User'

	And extract JSON from body
	
	Then HTTP status code equals to '200'
	Then JSON 'id'='User'
	Then JSON 'name'='User'
	Then JSON 'description'='User Account'
	Then JSON 'endpoint'='/Users'
	Then JSON 'schema'='urn:ietf:params:scim:schemas:core:2.0:User'
	Then JSON 'schemaExtensions[0].required'='true'
	Then JSON 'schemaExtensions[0].schema'='urn:ietf:params:scim:schemas:extension:enterprise:2.0:User'
	Then JSON 'meta.resourceType'='ResourceType'