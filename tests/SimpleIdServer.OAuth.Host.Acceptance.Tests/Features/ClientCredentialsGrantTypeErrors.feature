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

Scenario: Send 'grant_type=client_credentials,scope=scope' with no client_id
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key        | Value              |
	| grant_type | client_credentials |
	| scope      | scope  	          |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='missing client_id'	

Scenario: Send 'grant_type=client_credentials,scope=scope,client_id=c' with invalid client_id
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key        | Value              |
	| grant_type | client_credentials |
	| scope      | scope  	          |
	| client_id  | c                  |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='unknown client c'
	
Scenario: Send 'grant_type=client_credentials,scope=scope,client_id=firstClient' with invalid no client_secret
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key        | Value              |
	| grant_type | client_credentials |
	| scope      | scope  	          |
	| client_id  | firstClient        |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='bad client credential'
	
Scenario: Send 'grant_type=client_credentials,scope=scope,client_id=firstClient,client_secret=bad' with invalid client_secret
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value              |
	| grant_type    | client_credentials |
	| scope         | scope  	         |
	| client_id     | firstClient        |
	| client_secret | bad                |

	And extract JSON from body
	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_client'
	And JSON '$.error_description'='bad client credential'
	
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