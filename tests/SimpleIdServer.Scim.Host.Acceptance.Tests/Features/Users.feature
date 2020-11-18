Feature: Users
	Check the /Users endpoint

Scenario: Check User can be created
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| externalId     | externalid                                                                                                     |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |
	| type           | manager                                                                                                        |
	| age            | 22                                                                                                             |
	| eidCertificate | aGVsbG8=                                                                                                       |
	
	And extract JSON from body

	Then HTTP status code equals to '201'	
	Then HTTP HEADER contains 'Location'
	Then HTTP HEADER contains 'ETag'
	Then JSON exists 'id'
	Then JSON exists 'meta.created'
	Then JSON exists 'meta.lastModified'
	Then JSON exists 'meta.version'
	Then JSON exists 'meta.location'
	Then JSON 'employeeNumber'='number'
	Then JSON 'externalId'='externalid'
	Then JSON 'meta.resourceType'='Users'
	Then JSON 'userName'='bjen'
	Then JSON 'name.formatted'='formatted'
	Then JSON 'name.familyName'='familyName'
	Then JSON 'name.givenName'='givenName'
	Then JSON 'org'='ENTREPRISE'
	Then JSON 'age'='22'
	Then JSON 'eidCertificate'='aGVsbG8='

Scenario: Check attribute with a mutability equals to readOnly cannot be overriden
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| groups         | [ { "value": "group" } ]                                                                                       |
	| employeeNumber | number                                                                                                         |

	And extract JSON from body

	Then HTTP status code equals to '201'	
	Then HTTP HEADER contains 'Location'
	Then HTTP HEADER contains 'ETag'
	Then JSON doesn't exists 'groups'

Scenario: Check User can be returned
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |
	
	And extract JSON from body
	And extract 'id' from JSON body
	And execute HTTP GET request 'http://localhost/Users/$id$'
	And extract JSON from body

	Then HTTP status code equals to '200'
	Then HTTP HEADER contains 'Location'
	Then HTTP HEADER contains 'ETag'

Scenario: Check user can be removed
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| employeeNumber | number                                                                                                         |
	
	And extract JSON from body
	And extract 'id' from JSON body
	When execute HTTP DELETE request 'http://localhost/Users/$id$'
	
	Then HTTP status code equals to '204'

Scenario: Check user can be filtered (HTTP GET)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| phones         | [ { "phoneNumber": "01", "type": "mobile" }, { "phoneNumber": "02", "type": "home" } ]                         |
	| employeeNumber | number                                                                                                         |

	And execute HTTP GET request 'http://localhost/Users?filter=userName%20eq%20bjen&count=3'	
	And extract JSON from body

	Then HTTP status code equals to '200'
	Then JSON exists 'Resources[0].phones'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:ListResponse'
	Then JSON 'totalResults'='1'
	Then JSON 'startIndex'='1'
	Then JSON 'itemsPerPage'='3'
	Then JSON 'Resources[0].userName'='bjen'
	Then JSON 'Resources[0].name.formatted'='formatted'
	Then JSON 'Resources[0].name.familyName'='familyName'
	Then JSON 'Resources[0].name.givenName'='givenName'

Scenario: Check user can be filtered (HTTP POST)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| phones         | [ { "phoneNumber": "01", "type": "mobile" }, { "phoneNumber": "02", "type": "home" } ]                         |
	| employeeNumber | number                                                                                                         |
	| age            | 22                                                                                                             |
	| eidCertificate | aGVsbG8=                                                                                                       |

	And execute HTTP POST JSON request 'http://localhost/Users/.search'	
	| Key        | Value                                                                  |
	| filter     | userName eq "bjen" and ( age gt 15" and eidCertificate eq "aGVsbG8=" ) |
	| count      | 3                                                                      |
	| attributes | [ 'phones.phoneNumber' ]                                               |

	And extract JSON from body

	Then HTTP status code equals to '200'
	Then JSON exists 'Resources[0].phones'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:ListResponse'
	Then JSON 'totalResults'='1'
	Then JSON 'startIndex'='1'
	Then JSON 'itemsPerPage'='3'
	Then JSON 'Resources[0].phones[0].phoneNumber'='01'
	Then JSON 'Resources[0].phones[1].phoneNumber'='02'
	Then JSON exists 'Resources[0].id'

Scenario: Check the maxResults option is used when the client is trying to fetch more than the allowed number of data
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| phones         | [ { "phoneNumber": "01", "type": "mobile" }, { "phoneNumber": "02", "type": "home" } ]                         |
	| employeeNumber | number                                                                                                         |

	And execute HTTP GET request 'http://localhost/Users?filter=userName%20eq%20bjen&count=300'	
	And extract JSON from body

	Then HTTP status code equals to '200'
	Then JSON 'itemsPerPage'='200'
	
Scenario: Use 'attributes=phones.phoneNumber' to get only the phone numbers
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| phones         | [ { "phoneNumber": "01", "type": "mobile" }, { "phoneNumber": "02", "type": "home" } ]                         |
	| employeeNumber | number                                                                                                         |

	And execute HTTP GET request 'http://localhost/Users?filter=userName%20eq%20bjen&count=3&attributes=phones.phoneNumber'	
	And extract JSON from body

	Then HTTP status code equals to '200'
	Then JSON exists 'Resources[0].phones'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:ListResponse'
	Then JSON 'totalResults'='1'
	Then JSON 'startIndex'='1'
	Then JSON 'itemsPerPage'='3'
	Then JSON 'Resources[0].phones[0].phoneNumber'='01'
	Then JSON 'Resources[0].phones[1].phoneNumber'='02'
	Then JSON exists 'Resources[0].id'

Scenario: Exclude parameters phone.phoneNumber, userName and id
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| phones         | [ { "phoneNumber": "01", "type": "mobile" }, { "phoneNumber": "02", "type": "home" } ]                         |
	| employeeNumber | number                                                                                                         |

	And execute HTTP GET request 'http://localhost/Users?filter=userName%20eq%20bjen&count=3&excludedAttributes=phones.phoneNumber,userName,id'	
	And extract JSON from body

	Then HTTP status code equals to '200'
	Then JSON exists 'Resources[0].phones'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:ListResponse'
	Then JSON 'totalResults'='1'
	Then JSON 'startIndex'='1'
	Then JSON 'itemsPerPage'='3'
	Then JSON doesn't exists 'Resources[0].userName'
	Then JSON doesn't exists 'Resources[0].phones[0].phoneNumber'
	Then JSON doesn't exists 'Resources[0].phones[1].phoneNumber'
	Then JSON exists 'Resources[0].id'

Scenario: Check user can be updated (HTTP PUT)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| phones         | [ { "phoneNumber": "01", "type": "mobile" }, { "phoneNumber": "02", "type": "home" } ]                         |
	| employeeNumber | number                                                                                                         |
	| externalId     | ext                                                                                                            |
	
	And extract JSON from body
	And extract 'id' from JSON body	
	And execute HTTP PUT JSON request 'http://localhost/Users/$id$'
	| Key        | Value                                                                                        |
	| schemas    | [ "urn:ietf:params:scim:schemas:core:2.0:User" ]                                             |
	| name       | { "formatted" : "newFormatted", "familyName": "newFamilyName", "givenName": "newGivenName" } |
	| id         | $id$                                                                                         |
	| externalId | newext                                                                                       |
	
	And execute HTTP GET request 'http://localhost/Users/$id$'	
	And extract JSON from body

	Then HTTP status code equals to '200'
	Then HTTP HEADER contains 'Location'
	Then HTTP HEADER contains 'ETag'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:schemas:core:2.0:User'
	Then JSON 'userName'='bjen'
	Then JSON 'externalId'='newext'
	Then JSON 'name.formatted'='newFormatted'
	Then JSON 'name.familyName'='newFamilyName'
	Then JSON 'name.givenName'='newGivenName'

Scenario: Check user can be patched (HTTP PATCH)
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                                                                                      |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ]                                                             |
	| userName       | bjen                                                                                                                                                                       |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                                                                                        |
	| phones         | [ { "phoneNumber": "01", "type": "mobile" }, { "phoneNumber": "02", "type": "home" }, { "phoneNumber": "04", "type": "home" }, { "phoneNumber": "05", "type": "home05" } ] |
	| employeeNumber | number                                                                                                                                                                     |
	| scores         | { "math" : [ { "score" : "10" } ] }                                                                                                                                        |
	| roles          | [ "role1", "role2" ]                                                                                                                                                       |

	And extract JSON from body
	And extract 'id' from JSON body	
	And execute HTTP PATCH JSON request 'http://localhost/Users/$id$'
	| Key        | Value                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            |
	| schemas    | [ "urn:ietf:params:scim:api:messages:2.0:PatchOp" ]                                                                                                                                                                                                                                                                                                                                                                                                                                              |
	| Operations | [ { "op": "remove", "path": "phones[phoneNumber eq 05].phoneNumber" }, { "op" : "replace", "path": "phones[phoneNumber eq 04]", "value": { "type": "home" } }, { "op" : "remove", "path": "phones[phoneNumber eq 01]" }, { "op": "add", "path": "phones", "value": { "phoneNumber": "03", "type": "mobile" } }, { "op": "replace", "path": "userName", "value": "cassandra" }, { "op" : "remove", "path": "scores.math[score eq \"10\"]" }, { "op" : "add", "path": "scores.math", "value": { "score": "20" } }, { "op": "add", "path": "roles", "value": "role3" } ] |
	
	And execute HTTP GET request 'http://localhost/Users/$id$'	
	And extract JSON from body

	Then HTTP status code equals to '200'
	Then HTTP HEADER contains 'Location'
	Then HTTP HEADER contains 'ETag'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:schemas:core:2.0:User'
	Then JSON 'userName'='cassandra'
	Then JSON 'phones[0].phoneNumber'='02'
	Then JSON 'phones[0].type'='home'
	Then JSON 'phones[1].type'='home05'
	Then JSON doesn't exists 'phones[1].phoneNumber'
	Then JSON 'phones[2].type'='home'
	Then JSON 'phones[2].phoneNumber'='04'
	Then JSON 'phones[3].type'='mobile'
	Then JSON 'phones[3].phoneNumber'='03'
	Then JSON 'scores.math[0].score'='20'
	Then JSON 'roles[0]'='role1'
	Then JSON 'roles[1]'='role2'
	Then JSON 'roles[2]'='role3'

Scenario: Check no user is returned when count parameter is 0
	When execute HTTP POST JSON request 'http://localhost/Users'
	| Key            | Value                                                                                                          |
	| schemas        | [ "urn:ietf:params:scim:schemas:core:2.0:User", "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" ] |
	| userName       | bjen                                                                                                           |
	| name           | { "formatted" : "formatted", "familyName": "familyName", "givenName": "givenName" }                            |
	| phones         | [ { "phoneNumber": "01", "type": "mobile" }, { "phoneNumber": "02", "type": "home" } ]                         |
	| employeeNumber | number                                                                                                         |
	| scores         | { "math" : [ { "score" : "10" } ] }                                                                            |
	| roles          | [ "role1", "role2" ]                                                                                           |

	And execute HTTP GET request 'http://localhost/Users?count=0'	
	And extract JSON from body
	

	Then HTTP status code equals to '200'
	Then JSON 'schemas[0]'='urn:ietf:params:scim:api:messages:2.0:ListResponse'
	Then JSON 'totalResults'='1'
	Then JSON 'startIndex'='1'
	Then JSON 'itemsPerPage'='0'