Feature: AuthorizationCallbackErrors
	Check errors returned by the callback operation of the authorization api
	
Scenario: IdToken parameter is required
	When execute HTTP POST request 'https://localhost:8080/authorization/callback'
	| Key        | Value    |
	
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing parameter id_token'

Scenario: IdToken parameter must be a valid JSON Web Token
	When execute HTTP POST request 'https://localhost:8080/authorization/callback'
	| Key        | Value    |
	| id_token   | bad      |
	
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='JSON Web Token cannot be read'

Scenario: Nonce claim must be present in the JSON Web Token
	Given build JWS id_token_hint and sign with the key 'keyid'
	| Key | Value     |
	| sub | otheruser |

	When execute HTTP POST request 'https://localhost:8080/authorization/callback'
	| Key        | Value           |
	| id_token   | $id_token_hint$ |	
	
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='the nonce claim is missing in the id_token'

Scenario: Nonce must be valid
	Given build JWS id_token_hint and sign with the key 'keyid'
	| Key   | Value        |
	| sub   | otheruser    |
	| nonce | invalidnonce |

	When execute HTTP POST request 'https://localhost:8080/authorization/callback'
	| Key        | Value           |
	| id_token   | $id_token_hint$ |	
	
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='the nonce is invalid and doesn't match the nonce present in the authorization request'

Scenario: Identity token must be signed by the Distributed Identity Document
	Given build authorization request callback 'nonce'

	And build JWS id_token_hint and sign with the key 'keyid'
	| Key   | Value        |
	| sub   | otheruser    |
	| nonce | nonce        |

	When execute HTTP POST request 'https://localhost:8080/authorization/callback'
	| Key        | Value           |
	| id_token   | $id_token_hint$ |	
	
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='did doesn't have the correct format'