Feature: CredentialErrors
	Check errors returned by the credential endpoint
	
Scenario: access token is required
	When execute HTTP POST JSON request 'https://localhost:8080/credential'
	| Key           | Value                 |

	And extract JSON from body
	
	Then JSON 'error'='access_denied'
	And JSON 'error_description'='missing token'
	
Scenario: access token must be valid
	When execute HTTP POST JSON request 'https://localhost:8080/credential'
	| Key           | Value                 |
	| Authorization | Bearer INVALID        |

	And extract JSON from body

	Then HTTP status code equals to '401'
	And JSON 'error'='invalid_token'
	And JSON 'error_description'='either the access token has been revoked or is invalid'

Scenario: format is required

Scenario: types is required

Scenario: consent must has been given to the client (authorization_code)

Scenario: consent must has been given to the client (pre-authorized_code)