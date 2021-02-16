Feature: Jwks
	Get the JWKS and check its content

Scenario: Get the jwks
	When execute HTTP GET request 'https://localhost:8080/jwks'
	| Key			| Value													|

	Then HTTP status code equals to '200'