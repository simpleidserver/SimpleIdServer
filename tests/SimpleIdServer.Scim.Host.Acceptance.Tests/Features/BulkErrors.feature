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