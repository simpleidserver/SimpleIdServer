Feature: IntrospectErrors
	Check errors returned during the token introspection

Scenario: Error is returned when token is missing
	When execute HTTP POST request 'https://localhost:8080/token_info'
	| Key | Value |
	
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_request'
	And JSON '$.error_description'='missing parameter token'