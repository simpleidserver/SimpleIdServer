Feature: TokenErrors
	Check errors returned by the token endpoint

Scenario: Send an empty request
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key | Value |
	
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_grant'
	And JSON '$.error_description'='bad grant type'

Scenario: authorization cannot used twice
	Given authenticate a user
	When execute HTTP GET request 'https://localhost:8080/authorization'
	| Key           | Value                 |
	| response_type | code                  |
	| client_id     | thirdClient           |
	| state         | state                 |
	| redirect_uri  | http://localhost:8080 |
	| response_mode | query                 |
	| scope         | secondScope			|

	And extract parameter 'code' from redirect url
	And extract parameter 'state' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			|
	| client_id     | thirdClient			|
	| client_secret | password     			|
	| grant_type    | authorization_code	|
	| code			| $code$				|	
	| redirect_uri  | http://localhost:8080	|	
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			|
	| client_id     | thirdClient			|
	| client_secret | password     			|
	| grant_type    | authorization_code	|
	| code			| $code$				|	
	| redirect_uri  | http://localhost:8080	|	
	
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_grant'
	And JSON '$.error_description'='authorization code has already been used, all tokens previously issued have been revoked'