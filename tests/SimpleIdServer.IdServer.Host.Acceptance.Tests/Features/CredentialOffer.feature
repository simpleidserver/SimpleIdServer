Feature: CredentialOffer
	Check result returned by credential_offer API
	
Scenario: get a credential_offer url
	Given authenticate a user
	
	When execute HTTP GET request 'http://localhost/authorization'
	| Key                     | Value                   |
	| response_type           | code                    |
	| client_id               | fiftyEightClient        |
	| state                   | state                   |
	| response_mode           | query                   |
	| redirect_uri            | http://localhost:8080   |
	| nonce                   | nonce                   |
	| scope                   | credential_offer        |

	And extract parameter 'code' from redirect url
	
	And execute HTTP POST request 'https://localhost:8080/token'
	| Key           | Value        			|
	| client_id     | fiftyEightClient      |
	| client_secret | password     			|
	| grant_type    | authorization_code	|
	| code			| $code$				|
	| redirect_uri  | http://localhost:8080	|
	
	And extract JSON from body
	And extract parameter '$.access_token' from JSON body into 'accessToken'

	And execute HTTP GET request 'http://localhost/credential_offer/credentialOfferId'
	| Key           | Value                 |
	| Authorization | Bearer $accessToken$  |

	And extract query parameters into JSON

	Then JSON exists 'credential_offer'