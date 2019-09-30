Feature: Users
	Check the /Users endpoint

Scenario: Check User can be created
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key		| Value																									|
	| schemas	| [ "urn:ietf:params:scim:schemas:core:2.0:User" ]														|
	| userName	| bjen																									|
	| name		| { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }					|

	And extract JSON from body

	Then HTTP status code equals to '201'	
	Then JSON exists 'id'
	Then JSON exists 'meta.created'
	Then JSON exists 'meta.lastModified'
	Then JSON exists 'meta.version'
	Then JSON exists 'meta.location'
	Then JSON 'meta.resourceType'='Users'
	Then JSON 'userName'='bjen'
	Then JSON 'name.formatted'='formatted'
	Then JSON 'name.familyName'='familyName'
	Then JSON 'name.givenName'='givenName'