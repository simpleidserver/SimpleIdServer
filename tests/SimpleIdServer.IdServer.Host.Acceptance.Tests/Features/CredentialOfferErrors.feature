Feature: CredentialOfferErrors
	Check errors returned by credential_offer API
	
Scenario: access token is required
	When execute HTTP GET request 'http://localhost/credential_offer/id'
	| Key           | Value                 |

	And extract JSON from body
	
	Then JSON 'error'='access_denied'
	And JSON 'error_description'='missing token'

Scenario: accesss token is not valid
	When execute HTTP GET request 'http://localhost/credential_offer/id'
	| Key           | Value                 |
	| Authorization | Bearer AT             |

	And extract JSON from body
	
	Then JSON 'error'='invalid_token'
	And JSON 'error_description'='either the access token has been revoked or is invalid'
	
Scenario: cannot get an unknown credential offer
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

	And execute HTTP GET request 'http://localhost/credential_offer/unknownCredentialOffer'
	| Key           | Value                 |
	| Authorization | Bearer $accessToken$  |

	And extract JSON from body
	
	Then HTTP status code equals to '404'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='the credential offer doesn't exist'