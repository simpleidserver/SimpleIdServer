Feature: Introspect
	Check result returned during the token introspection

Scenario: IsActive is false when the token doesn't exist
	When execute HTTP POST request 'https://localhost:8080/token_info'
	| Key           | Value       |
	| client_id     | firstClient |
	| client_secret | password    |
	| token         | token       |
	
	And extract JSON from body

	Then HTTP status code equals to '200'
	And JSON '$.active'='false'

Scenario: IsActive is false when the token is expired
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value              |
	| client_id     | thirteenClient     |
	| client_secret | password           |
	| scope         | secondScope	     |
	| grant_type    | client_credentials |
	
	And extract JSON from body
	And extract parameter '$.access_token' from JSON body into 'accessToken'

	And execute HTTP POST request 'https://localhost:8080/token_info'
	| Key           | Value          |
	| client_id     | thirteenClient |
	| client_secret | password       |
	| token         | $accessToken$  |
	
	And extract JSON from body

	Then HTTP status code equals to '200'
	And JSON '$.active'='false'

Scenario: IsActive is true when the token is introspected
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value              |
	| client_id     | firstClient        |
	| client_secret | password           |
	| scope         | firstScope	     |
	| grant_type    | client_credentials |
	
	And extract JSON from body
	And extract parameter '$.access_token' from JSON body into 'accessToken'

	And execute HTTP POST request 'https://localhost:8080/token_info'
	| Key           | Value          |
	| client_id     | firstClient    |
	| client_secret | password       |
	| token         | $accessToken$  |
	
	And extract JSON from body

	Then HTTP status code equals to '200'
	And JSON '$.active'='true'
	And JSON '$.client_id'='firstClient'
	And JSON '$.scope'='firstScope'
	And JSON '$.iss'='https://localhost:8080'

Scenario: JKT is returned
	When build DPoP proof
	| Key | Value                          |
	| htm | POST                           |
	| htu | https://localhost:8080/token   |

	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value              |
	| client_id     | sixtyThreeClient	 |
	| client_secret | password           |
	| scope         | firstScope	     |
	| grant_type    | client_credentials |
	| DPoP          | $DPOP$             |
	
	And extract JSON from body
	And extract parameter '$.access_token' from JSON body into 'accessToken'

	And execute HTTP POST request 'https://localhost:8080/token_info'
	| Key           | Value               |
	| client_id     | sixtyThreeClient    |
	| client_secret | password            |
	| token         | $accessToken$       |
	
	And extract JSON from body

	Then HTTP status code equals to '200'
	And JSON '$.active'='true'
	And JSON '$.client_id'='sixtyThreeClient'
	And JSON '$.scope'='firstScope'
	And JSON '$.iss'='https://localhost:8080'
	And JSON exists '$.cnf.jkt'