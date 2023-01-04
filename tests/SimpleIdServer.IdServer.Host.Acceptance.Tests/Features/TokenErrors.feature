Feature: TokenErrors
	Check errors returned by the token endpoint

Scenario: Send an empty request
	When execute HTTP POST request 'https://localhost:8080/token'
	| Key | Value |
	
	And extract JSON from body

	Then HTTP status code equals to '400'
	And JSON '$.error'='invalid_grant'
	And JSON '$.error_description'='bad grant type'