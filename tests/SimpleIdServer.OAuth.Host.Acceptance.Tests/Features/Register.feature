Feature: Register
	Check client can be registered

Scenario: Register a client
	When execute HTTP POST request 'https://localhost:8080/register'
	| Key       | Value     |
	| client_id | newClient |
	
	And extract JSON from body

	Then HTTP status code equals to '201'

Scenario: Get a client
	When execute HTTP POST request 'https://localhost:8080/register'
	| Key       | Value      |
	| client_id | newClient2 |

	And extract JSON from body
	And extract parameter '$.registration_access_token' from JSON body into 'accessToken'

	And execute HTTP GET request 'https://localhost:8080/register/newClient2'
	| Key           | Value                |
	| Authorization | Bearer $accessToken$ |

	Then HTTP status code equals to '200'

Scenario: Update a client
	When execute HTTP POST request 'https://localhost:8080/register'
	| Key       | Value      |
	| client_id | newClient3 |

	And extract JSON from body
	And extract parameter '$.registration_access_token' from JSON body into 'accessToken'

	And execute HTTP PUT request 'https://localhost:8080/register/newClient3'
	| Key                        | Value                |
	| Authorization              | Bearer $accessToken$ |
	| client_id                  | newClient3           |
	| token_endpoint_auth_method | client_secret_basic  |	

	And extract JSON from body

	Then HTTP status code equals to '200'
	Then JSON '$.token_endpoint_auth_method'='client_secret_basic'