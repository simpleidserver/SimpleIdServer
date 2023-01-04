Feature: RevokeToken
	Revoke access token or refresh token

Scenario: Revoke access_token
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value              |
	| client_id     | firstClient		 |
	| client_secret | password           |
	| scope         | firstScope	     |
	| grant_type    | client_credentials |
	
	And extract JSON from body
	And extract parameter '$.access_token' from JSON body into 'accessToken'

	When execute HTTP POST request 'https://localhost:8080/token/revoke'
	| Key             | Value         |
	| client_id       | firstClient   |
	| client_secret   | password      |
	| token           | $accessToken$ |
	| token_type_hint | access_token  |

	Then HTTP status code equals to '200'