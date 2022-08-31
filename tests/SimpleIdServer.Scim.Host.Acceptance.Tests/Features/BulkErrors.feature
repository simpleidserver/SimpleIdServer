Feature: BulkErrors
	Check the errors returned by the /Bulk endpoint
		
Scenario: Error is returned when too many operations are passed 
	When execute HTTP POST JSON request 'http://localhost/Bulk'
	| Key			| Value																																																										|
	| schemas		| [ "urn:ietf:params:scim:api:messages:2.0:BulkRequest" ]																																													|
	| Operations	| [ { "method": "GET", "path": "/Users/id", "bulkId": "2" }, { "method": "GET", "path": "/Users/id", "bulkId": "2" }, { "method": "GET", "path": "/Users/id", "bulkId": "2" }, { "method": "GET", "path": "/Users/id", "bulkId": "2" } ]	|

	And extract JSON from body
	
	Then HTTP status code equals to '413'
	Then JSON 'status'='413'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:Error'
	Then JSON 'status'='413'
	Then JSON 'scimType'='tooLarge'
		
Scenario: Error is returned when bulkId it not well formatted
	When execute HTTP POST JSON request 'http://localhost/Bulk'
	| Key			| Value																																																																																																																																					|
	| schemas		| [ "urn:ietf:params:scim:api:messages:2.0:BulkRequest" ]																																																																																																																								|
	| Operations	| [ { "method": "POST", "path": "/Users", "bulkId": "1", "data": { "schemas": [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ], "employeeNumber": "number", "userName": "bjen", "name": { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" } } }, { "method": "POST", "path": "/Groups", "bulkId": "2", "data": { "schemas": [ "urn:ietf:params:scim:schemas:core:2.0:Group" ], "displayName": "Tour Guides", "members": [ { "value": "bulkId" } ] } } ]	|

	And extract JSON from body
	
	Then JSON 'Operations[1].bulkId'='2'
	Then JSON 'Operations[1].status'='400'
	Then JSON 'Operations[1].response.scimType'='invalidSyntax'
	Then JSON 'Operations[1].response.detail'='bulkId bulkId is not well formatted'
		
Scenario: Error is returned when bulkId doesn't exist
	When execute HTTP POST JSON request 'http://localhost/Bulk'
	| Key			| Value																																																																																																																																							|
	| schemas		| [ "urn:ietf:params:scim:api:messages:2.0:BulkRequest" ]																																																																																																																										|
	| Operations	| [ { "method": "POST", "path": "/Users", "bulkId": "1", "data": { "schemas": [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ], "employeeNumber": "number", "userName": "bjen", "name": { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" } } }, { "method": "POST", "path": "/Groups", "bulkId": "2", "data": { "schemas": [ "urn:ietf:params:scim:schemas:core:2.0:Group" ], "displayName": "Tour Guides", "members": [ { "value": "bulkId:invalid" } ] } } ]	|

	And extract JSON from body
	
	Then JSON 'Operations[1].bulkId'='2'
	Then JSON 'Operations[1].status'='400'
	Then JSON 'Operations[1].response.scimType'='invalidSyntax'
	Then JSON 'Operations[1].response.detail'='bulkId invalid doesn't exist'