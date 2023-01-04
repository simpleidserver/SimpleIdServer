Feature: RevokeTokenErrors
	Check errors returned when trying to revoke a token

Scenario: Error is returned when token is missing
	When execute HTTP POST request 'https://localhost:8080/token/revoke'
	| Key | Value |
	
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='missing parameter token'

Scenario: Error is returned when token_type_hint is invalid
	When execute HTTP POST request 'https://localhost:8080/token/revoke'
	| Key             | Value |
	| token           | token |
	| token_type_hint | bad   |

	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON '$.error'='unsupported_token_type'
	And JSON '$.error_description'='unknown token type hint : bad'