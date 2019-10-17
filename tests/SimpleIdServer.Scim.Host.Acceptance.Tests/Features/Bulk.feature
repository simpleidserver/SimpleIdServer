Feature: Bulk
	Check the /Bulk endpoint
		
Scenario: Check operations can be executed with BULK request
	When execute HTTP POST JSON request 'http://localhost/Bulk'
	| Key			| Value																																																																																		|
	| schemas		| [ "urn:ietf:params:scim:api:messages:2.0:BulkRequest" ]																																																																					|
	| Operations	| [ { "method": "POST", "path": "/Users", "bulkId": "1", "data": { "schemas": [ "urn:ietf:params:scim:schemas:core:2.0:User" ], "userName": "bjen", "name": { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" } } }, { "method": "GET", "path": "/Users/id", "bulkId": "2" } ]								|

	And extract JSON from body

	Then HTTP status code equals to '200'
	Then JSON exists 'Operations'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:BulkResponse'