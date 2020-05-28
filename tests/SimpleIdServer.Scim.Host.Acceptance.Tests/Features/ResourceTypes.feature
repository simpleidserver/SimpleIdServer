Feature: ResourceTypes
	Check the /ResourceTypes endpoint
		
Scenario: Check informations returned by ResourceTypes endpoint
	When execute HTTP GET request 'http://localhost/ResourceTypes'

	And extract JSON from body
	
	Then HTTP status code equals to '200'
	Then JSON '[0].id'='Users'
	Then JSON '[0].name'='User'
	Then JSON '[0].description'='User Account'
	Then JSON '[0].endpoint'='/Users'
	Then JSON '[0].schema'='urn:ietf:params:scim:schemas:core:2.0:User'
	Then JSON '[0].schemaExtensions[0].required'='true'
	Then JSON '[0].schemaExtensions[0].schema'='urn:ietf:params:scim:schemas:extension:enterprise:2.0:User'
	Then JSON '[0].meta.resourceType'='Users'
	Then JSON '[2].id'='Groups'
	Then JSON '[2].name'='Group'
	Then JSON '[2].description'='Group'
	Then JSON '[2].endpoint'='/Groups'
	Then JSON '[2].schema'='urn:ietf:params:scim:schemas:core:2.0:Group'
	Then JSON '[2].meta.resourceType'='Groups'