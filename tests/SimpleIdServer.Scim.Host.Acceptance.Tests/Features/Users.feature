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
	Then HTTP HEADER contains 'Location'
	Then HTTP HEADER contains 'ETag'
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
	Then JSON 'org'='ENTREPRISE'

Scenario: Check attribute with a mutability equals to readOnly cannot be overriden
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key      | Value																					|
	| schemas  | [ "urn:ietf:params:scim:schemas:core:2.0:User" ]										|
	| userName | bjen																					|
	| name     | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }	|
	| groups   | [ { "value": "group" } ]																|

	And extract JSON from body

	Then HTTP status code equals to '201'	
	Then HTTP HEADER contains 'Location'
	Then HTTP HEADER contains 'ETag'
	Then JSON doesn't exists 'groups'

Scenario: Check User can be returned
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key		| Value																									|
	| schemas	| [ "urn:ietf:params:scim:schemas:core:2.0:User" ]														|
	| userName	| bjen																									|
	| name		| { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }					|
	
	And extract JSON from body
	And extract 'id' from JSON body
	And execute HTTP GET request 'http://localhost/Users/$id$'
	And extract JSON from body

	Then HTTP status code equals to '200'
	Then HTTP HEADER contains 'Location'
	Then HTTP HEADER contains 'ETag'

Scenario: Check user can be removed
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key		| Value																									|
	| schemas	| [ "urn:ietf:params:scim:schemas:core:2.0:User" ]														|
	| userName	| bjen																									|
	| name		| { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }					|
	
	And extract JSON from body
	And extract 'id' from JSON body
	When execute HTTP DELETE request 'http://localhost/Users/$id$'
	
	Then HTTP status code equals to '204'

Scenario: Check user can be filtered
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key		| Value																									|
	| schemas	| [ "urn:ietf:params:scim:schemas:core:2.0:User" ]														|
	| userName	| bjen																									|
	| name		| { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }					|
	| phones	| [ { "phoneNumber": "01", "type": "mobile" }, { "phoneNumber": "02", "type": "home" } ]				|

	And execute HTTP GET request 'http://localhost/Users?filter=userName%20eq%20bjen&count=3'	
	And extract JSON from body

	Then HTTP status code equals to '200'
	Then JSON exists 'Resources[0].phones'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:ListResponse'
	Then JSON 'totalResults'='1'
	Then JSON 'startIndex'='0'
	Then JSON 'itemsPerPage'='3'
	Then JSON 'Resources[0].userName'='bjen'
	Then JSON 'Resources[0].name.formatted'='formatted'
	Then JSON 'Resources[0].name.familyName'='familyName'
	Then JSON 'Resources[0].name.givenName'='givenName'

Scenario: Check the maxResults option is used when the client is trying to fetch more than the allowed number of data
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key		| Value																									|
	| schemas	| [ "urn:ietf:params:scim:schemas:core:2.0:User" ]														|
	| userName	| bjen																									|
	| name		| { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }					|
	| phones	| [ { "phoneNumber": "01", "type": "mobile" }, { "phoneNumber": "02", "type": "home" } ]				|

	And execute HTTP GET request 'http://localhost/Users?filter=userName%20eq%20bjen&count=300'	
	And extract JSON from body

	Then HTTP status code equals to '200'
	Then JSON 'itemsPerPage'='200'


Scenario: Use 'attributes=phones.phoneNumber' to get only the phone numbers
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key		| Value																									|
	| schemas	| [ "urn:ietf:params:scim:schemas:core:2.0:User" ]														|
	| userName	| bjen																									|
	| name		| { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }					|
	| phones	| [ { "phoneNumber": "01", "type": "mobile" }, { "phoneNumber": "02", "type": "home" } ]				|

	And execute HTTP GET request 'http://localhost/Users?filter=userName%20eq%20bjen&count=3&attributes=phones.phoneNumber'	
	And extract JSON from body

	Then HTTP status code equals to '200'
	Then JSON exists 'Resources[0].phones'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:ListResponse'
	Then JSON 'totalResults'='1'
	Then JSON 'startIndex'='0'
	Then JSON 'itemsPerPage'='3'
	Then JSON 'Resources[0].phones[0].phoneNumber'='01'
	Then JSON 'Resources[0].phones[1].phoneNumber'='02'

Scenario: Check user can be updated (HTTP PUT)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key		| Value																									|
	| schemas	| [ "urn:ietf:params:scim:schemas:core:2.0:User" ]														|
	| userName	| bjen																									|
	| name		| { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }					|
	| phones	| [ { "phoneNumber": "01", "type": "mobile" }, { "phoneNumber": "02", "type": "home" } ]				|
	
	And extract JSON from body
	And extract 'id' from JSON body	
	And execute HTTP PUT JSON request 'http://localhost/Users/$id$'
	| Key     | Value																									|
	| schemas | [ "urn:ietf:params:scim:schemas:core:2.0:User" ]														|
	| name		| { "formatted" : "newFormatted", "familyName": "newFamilyName", "givenName": "newGivenName" }			|	
	
	And execute HTTP GET request 'http://localhost/Users/$id$'	
	And extract JSON from body

	Then HTTP status code equals to '200'
	Then HTTP HEADER contains 'Location'
	Then HTTP HEADER contains 'ETag'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:schemas:core:2.0:User'
	Then JSON 'userName'='bjen'
	Then JSON 'name.formatted'='newFormatted'
	Then JSON 'name.familyName'='newFamilyName'
	Then JSON 'name.givenName'='newGivenName'

Scenario: Check user can be patched (HTTP PATCH)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key		| Value																									|
	| schemas	| [ "urn:ietf:params:scim:schemas:core:2.0:User" ]														|
	| userName	| bjen																									|
	| name		| { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }					|
	| phones	| [ { "phoneNumber": "01", "type": "mobile" }, { "phoneNumber": "02", "type": "home" } ]				|

	And extract JSON from body
	And extract 'id' from JSON body	
	And execute HTTP PATCH JSON request 'http://localhost/Users/$id$'
	| Key     | Value																																																						|
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]																																										|
	| Operations | [ { "op" : "remove", "path": "phones[phoneNumber eq 01]" }, { "op": "add", "path": "phones", "value": { "phoneNumber": "03", "type": "mobile" } }, { "op": "replace", "path": "userName", "value": "cassandra" } ]		|
	
	And execute HTTP GET request 'http://localhost/Users/$id$'	
	And extract JSON from body

	Then HTTP status code equals to '200'
	Then HTTP HEADER contains 'Location'
	Then HTTP HEADER contains 'ETag'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:schemas:core:2.0:User'
	Then JSON 'userName'='cassandra'
	Then JSON 'phones[0].phoneNumber'='02'
	Then JSON 'phones[0].type'='home'
	Then JSON 'phones[1].phoneNumber'='03'
	Then JSON 'phones[1].type'='mobile'