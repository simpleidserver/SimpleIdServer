Feature: ClientCredentialsGrantTypeErrors
	Check errors returned when using 'client_credentials' grant-type

Scenario: Send 'grant_type=client_credentials' with no scope parameter
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key        | Value              |
	| grant_type | client_credentials |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='missing parameter scope'
	
Scenario: Send 'grant_type=client_credentials,scope=scope,client_id=secondClient,client_secret=password' with unauthorized grant-type
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value              |
	| grant_type    | client_credentials |
	| scope         | scope  	         |
	| client_id     | secondClient       |
	| client_secret | password           |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='grant type client_credentials is not supported by the client'